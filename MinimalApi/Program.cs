using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


builder.Services.AddDbContext<DbContexto> (options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))

    );
});



app.MapGet("/", () => "Hello World!");

// CORREÇÃO 1: A sintaxe do parâmetro foi corrigida para (Tipo nome).
app.MapPost("/login", (AspNetMinimalsApi.DTOs.LoginDTO loginDto) =>
{
    // ALERTA DE SEGURANÇA: Esta forma de verificar a senha é insegura e serve apenas para demonstração.
    // O correto seria usar um hash de senha armazenado em um banco de dados.
    if (loginDto.Email == "adm@teste.com" && loginDto.Senha == "123456")
    {
        return Results.Ok("Login efetuado com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
});


app.Run();

// BOA PRÁTICA: A definição da classe foi movida para antes de "app.Run()"
// e o nome segue a convenção PascalCase.
