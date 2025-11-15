# AspNetMinimalApi

Projeto Minimal API (.NET 8) com EF Core (MySQL), JWT e Swagger.

## Requisitos

- .NET SDK 8.0
- MySQL em execução e acessível
- Porta de execução default: `http://localhost:5162`

## Configuração

- Ajuste sua connection string em `minimal-api/appsettings.json` em `ConnectionStrings:mysql`.
- JWT em `minimal-api/appsettings.json`:
  - `Jwt.Key`: chave secreta (>= 32 chars recomendável)
  - `Jwt.Issuer`, `Jwt.Audience`, `Jwt.ExpiresMinutes`

Exemplo (trecho):

```
{
  "ConnectionStrings": {
    "mysql": "server=localhost;database=lab_minimal;user=root;password=123456"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_AQUI_TAMANHO_FORTE",
    "Issuer": "minimal-api",
    "Audience": "minimal-api-client",
    "ExpiresMinutes": 120
  }
}
```

## Banco de dados (Migrations)

Opcional (se ainda não tiver o schema criado):

```powershell
# Instalar CLI EF (se necessário)
dotnet tool install --global dotnet-ef

# Aplicar migrations
cd c:\AspNetMinimalsApi
 dotnet ef database update --project .\minimal-api\minimal-api.csproj
```

## Executar a API

```powershell
cd c:\AspNetMinimalsApi
 dotnet run --project .\minimal-api\minimal-api.csproj
```

A API sobe em `http://localhost:5162`. Swagger: `http://localhost:5162/swagger`.

## Fluxo de Autenticação

1. Login para obter o token:

- POST `/auth/login`
- Body JSON:

```
{"email":"administrador@teste.com","senha":"123456"}
```

Resposta (exemplo):

```
{
  "access_token": "<TOKEN>",
  "expires": "2025-11-15T12:00:00Z",
  "role": "Adm"
}
```

2. Enviar o token

- Header: `Authorization: Bearer <TOKEN>`

No Swagger: clique em "Authorize" e informe `Bearer <TOKEN>`.

## Endpoints principais

- `GET /` (Home)
- `POST /auth/login` (gera JWT)
- `GET /veiculos` (autenticado; filtros: `?pagina=1&nome=...&marca=...`)
- `GET /veiculos/{id}` (autenticado)
- `POST /veiculos` (Adm ou Editor)
- `PUT /veiculos/{id}` (Adm ou Editor)
- `DELETE /veiculos/{id}` (Adm)

## Regras de validação de Veículo

- `Nome`: obrigatório
- `Marca`: obrigatória
- `Ano`: entre 1900 e o ano atual + 1

## Exemplos cURL

Login:

```bash
curl -X POST http://localhost:5162/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"administrador@teste.com","senha":"123456"}'
```

Listar veículos:

```bash
curl http://localhost:5162/veiculos \
  -H "Authorization: Bearer <TOKEN>"
```

Criar veículo:

```bash
curl -X POST http://localhost:5162/veiculos \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Civic","marca":"Honda","ano":2022}'
```

## Perfis e Políticas

- `AdmOnly`: apenas Administrador (DELETE)
- `AdmOrEditor`: Administrador ou Editor (POST/PUT)
- Demais (GET): apenas autenticado

## Troubleshooting

- Erro de build com `Microsoft.OpenApi.Models` ou lock do `Microsoft.OpenApi.dll`:
  1. Feche terminais/prompt que estejam executando a API.
  2. Faça um build limpo:

```powershell
cd c:\AspNetMinimalsApi
 Remove-Item .\minimal-api\bin -Recurse -Force; Remove-Item .\minimal-api\obj -Recurse -Force
 dotnet clean .\AspNetMinimalsApi.sln
 dotnet restore .\minimal-api\minimal-api.csproj
 dotnet build .\AspNetMinimalsApi.sln -c Debug
```

3. Rode novamente:

```powershell
 dotnet run --project .\minimal-api\minimal-api.csproj
```

- 401/403: verifique se o header `Authorization` está sendo enviado com `Bearer <TOKEN>` e se o usuário tem perfil adequado.

---

Pronto! Esse README cobre execução, autenticação JWT, uso do Swagger/Authorize, CRUD de Veículos, regras e solução de problemas comuns.
