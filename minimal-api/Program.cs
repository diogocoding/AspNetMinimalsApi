using AspNetMinimalsApi.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using AspNetMinimalsApi.Dominio.Interfaces;
using AspNetMinimalsApi.DTOs;
using AspNetMinimalsApi.Infraestrutura.Db;
using AspNetMinimalsApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

// AuthN/AuthZ - JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection.GetValue<string>("Key")!;
var issuer = jwtSection.GetValue<string>("Issuer")!;
var audience = jwtSection.GetValue<string>("Audience")!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdmOnly", p => p.RequireRole("Adm"));
    options.AddPolicy("AdmOrEditor", p => p.RequireRole("Adm", "Editor"));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Home JSON
app.MapGet("/", () => Results.Ok(new { name = "AspNetMinimalApi", version = "v1" }))
    .WithTags("Home");

// CRUD Veículos
app.MapPost("/veiculos", (Veiculo veiculo, IVeiculoServico veiculoServico) =>
{
    var errors = ValidateVeiculo(veiculo);
    if (errors is not null)
        return Results.ValidationProblem(errors);
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
})
.RequireAuthorization("AdmOrEditor")
.WithTags("Veiculos");

app.MapGet("/veiculos", (int? pagina, string? nome, string? marca, IVeiculoServico veiculoServico) =>
{
    var page = pagina.GetValueOrDefault(1);
    return Results.Ok(veiculoServico.Todos(page, nome, marca));
})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    return veiculo is not null ? Results.Ok(veiculo) : Results.NotFound();
})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}", (int id, Veiculo veiculo, IVeiculoServico veiculoServico) =>
{
    if (id != veiculo.Id)
        return Results.BadRequest();
    veiculoServico.Atualizar(veiculo);
    return Results.NoContent();
})
.RequireAuthorization("AdmOrEditor")
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo is null)
        return Results.NotFound();
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
})
.RequireAuthorization("AdmOnly")
.WithTags("Veiculos");

// Auth: retorna JWT
app.MapPost("/auth/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm is null)
        return Results.Unauthorized();

    var claims = new[]
    {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, adm.Email),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, adm.Perfil)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpiresMinutes"));

    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { access_token = tokenString, expires = expires, role = adm.Perfil });
})
.WithTags("Auth");

// Helper de validação
static Dictionary<string, string[]>? ValidateVeiculo(Veiculo v)
{
    var errors = new Dictionary<string, string[]>();
    if (string.IsNullOrWhiteSpace(v.Nome))
        errors["Nome"] = new[] { "Nome é obrigatório." };
    if (string.IsNullOrWhiteSpace(v.Marca))
        errors["Marca"] = new[] { "Marca é obrigatória." };
    var year = DateTime.UtcNow.Year + 1;
    if (v.Ano < 1900 || v.Ano > year)
        errors["Ano"] = new[] { $"Ano deve estar entre 1900 e {year}." };
    return errors.Count > 0 ? errors : null;
}

app.Run();
