# 🚗 AspNetMinimalApi - Sistema de Gerenciamento de Veículos

**API RESTful moderna** desenvolvida com **.NET 8 Minimal APIs**, utilizando **Entity Framework Core**, **MySQL**, **autenticação JWT** e documentação **Swagger**. Este projeto implementa um CRUD completo de veículos com autorização baseada em roles (perfis de usuário).

---

## 📚 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Conceitos e Tecnologias](#-conceitos-e-tecnologias)
- [Arquitetura](#-arquitetura)
- [Funcionalidades](#-funcionalidades)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração](#-configuração)
- [Executar a API](#-executar-a-api)
- [Como Usar](#-como-usar)
- [Endpoints da API](#-endpoints-da-api)
- [Regras de Negócio](#-regras-de-negócio)
- [Segurança](#-segurança)
- [Exemplos de Uso](#-exemplos-de-uso)

---

## 🎯 Sobre o Projeto

Esta API foi desenvolvida como projeto educacional para demonstrar a implementação de uma **Minimal API** em .NET 8, integrando conceitos modernos de desenvolvimento web, segurança e persistência de dados. O sistema permite o gerenciamento completo de veículos (cadastro, consulta, atualização e exclusão) com controle de acesso baseado em autenticação JWT.

**Objetivo:** Criar uma API leve, performática e segura para gerenciar um catálogo de veículos, aplicando boas práticas de desenvolvimento e arquitetura de software.

---

## 🛠 Conceitos e Tecnologias

### Minimal APIs (.NET 8)

As **Minimal APIs** são uma abordagem simplificada para criar APIs HTTP com o mínimo de código e configuração. Diferentemente do modelo MVC tradicional, as Minimal APIs permitem definir endpoints diretamente no arquivo `Program.cs`, reduzindo a cerimônia e acelerando o desenvolvimento de microsserviços e APIs simples.

**Por que usar?**

- ✅ Menos boilerplate code
- ✅ Performance otimizada
- ✅ Ideal para microsserviços
- ✅ Sintaxe moderna e concisa
- ✅ Inicialização mais rápida

**Como funciona:**

```csharp
app.MapGet("/veiculos", (IVeiculoServico servico) => {
    return Results.Ok(servico.Todos());
});
```

Sem Controllers, sem ActionResults complexos - apenas funções mapeadas para rotas HTTP.

### Entity Framework Core (EF Core)

**ORM (Object-Relational Mapper)** que permite trabalhar com banco de dados usando objetos .NET, eliminando a necessidade de escrever SQL manualmente na maioria dos casos.

**Recursos utilizados:**

- **Code First**: Models C# definem a estrutura do banco
- **Migrations**: Versionamento de alterações no schema do banco
- **DbContext**: Contexto de conexão e configuração do banco
- **Pomelo.EntityFrameworkCore.MySql**: Provider específico para MySQL

**Exemplo de uso:**

```csharp
// Buscar veículo por ID sem escrever SQL
var veiculo = _contexto.Veiculos.Find(id);
```

**Vantagens:**

- Type-safe queries (erros em tempo de compilação)
- Menos código SQL manual
- Migrations automáticas
- Suporte a LINQ

### JSON Web Token (JWT)

Sistema de **autenticação stateless** baseado em tokens assinados digitalmente. O token contém claims (informações) sobre o usuário e não requer armazenamento no servidor.

**Estrutura do JWT:**

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9    ← Header (algoritmo)
.eyJodHRwOi8vc2NoZW1hcy54bWxzb2Fw...    ← Payload (dados/claims)
.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV     ← Signature (assinatura)
```

**Claims utilizadas neste projeto:**

- `Name`: Email do usuário
- `Role`: Perfil (Adm, Editor)

**Vantagens:**

- 🔐 Stateless (não precisa consultar banco a cada requisição)
- 📦 Auto-contido (informações no próprio token)
- 🔒 Seguro com assinatura HMAC-SHA256
- 🚀 Escalável (ideal para microserviços)

**Fluxo de autenticação:**

1. Cliente faz login com email/senha
2. API valida credenciais e gera JWT
3. Cliente armazena token
4. Cliente envia token no header `Authorization` em requisições subsequentes
5. API valida assinatura e extrai claims do token

### Swagger/OpenAPI

Ferramenta de **documentação interativa** que gera automaticamente uma interface web para testar os endpoints da API.

**Benefícios:**

- 📖 Documentação sempre atualizada com o código
- 🧪 Interface de teste integrada
- 📋 Definição padronizada (OpenAPI Specification)
- 🤝 Facilita comunicação entre frontend e backend

---

## 🏗 Arquitetura

O projeto segue uma **arquitetura em camadas** simplificada, promovendo separação de responsabilidades:

```
AspNetMinimalsApi/
├── Program.cs              # 🎯 Configuração, DI e definição de endpoints
├── appsettings.json        # ⚙️ Configurações (conexão, JWT)
├── Dominio/                # 📦 Camada de domínio (regras de negócio)
│   ├── Entidades/          # 🗂️ Models do banco de dados
│   │   ├── Administrador.cs
│   │   └── Veiculo.cs
│   ├── DTOs/               # 📨 Data Transfer Objects
│   │   └── LoginDTO.cs
│   ├── Interfaces/         # 📝 Contratos de serviços
│   │   ├── IAdministradorServico.cs
│   │   └── IVeiculoServico.cs
│   └── Servicos/           # ⚙️ Implementação da lógica de negócio
│       ├── AdministradorServico.cs
│       └── VeiculoServico.cs
├── Infraestrutura/         # 🔧 Camada de infraestrutura
│   └── Db/
│       └── DbContexto.cs   # 💾 Contexto EF Core
└── Migrations/             # 📊 Versionamento do banco de dados
    ├── *_AdminstrdorMigration.cs
    ├── *_SeedAdministrador.cs
    └── *_VeiculosMigration.cs
```

### Responsabilidades das Camadas:

1. **Apresentação (Program.cs)**

   - Configura serviços (DI)
   - Define endpoints HTTP
   - Aplica validações de entrada
   - Gerencia autenticação/autorização

2. **Domínio**

   - Define entidades de negócio
   - Interfaces de serviços (contratos)
   - Regras de negócio e validações
   - DTOs para transferência de dados

3. **Serviços**

   - Implementa lógica de aplicação
   - Orquestra operações CRUD
   - Aplica regras de negócio complexas

4. **Infraestrutura**
   - Acesso ao banco de dados
   - Configuração EF Core
   - Seed de dados iniciais

**Vantagens desta arquitetura:**

- ✅ Testabilidade (cada camada pode ser testada isoladamente)
- ✅ Manutenibilidade (mudanças localizadas)
- ✅ Reutilização de código
- ✅ Separação clara de responsabilidades

---

## ⚙️ Funcionalidades

### 1️⃣ Autenticação JWT

- 🔐 Login com email e senha
- 🎫 Geração de token JWT com claims personalizadas
- ✅ Validação automática de tokens em endpoints protegidos
- ⏰ Expiração configurável de tokens (default: 120 minutos)
- 🔒 Assinatura com HMAC-SHA256

### 2️⃣ Autorização por Roles (Perfis)

- **AdmOnly**: Apenas administradores (ex: deletar veículos)
- **AdmOrEditor**: Admins e editores (ex: criar/atualizar veículos)
- **Autenticado**: Qualquer usuário logado (ex: listar veículos)

| Perfil          | Permissões                          |
| --------------- | ----------------------------------- |
| **Adm**         | Todas (CRUD completo)               |
| **Editor**      | Criar, atualizar, listar veículos   |
| **Autenticado** | Apenas listar e visualizar veículos |

### 3️⃣ CRUD Completo de Veículos

- ✅ **Create**: Cadastrar novos veículos com validação
- ✅ **Read**: Listar todos ou buscar por ID (com filtros e paginação)
- ✅ **Update**: Atualizar dados de um veículo existente
- ✅ **Delete**: Remover veículos do sistema

### 4️⃣ Validações Robustas

- ✔️ Validação de dados de entrada (nome, marca, ano)
- ✔️ Respostas HTTP padronizadas (200, 201, 400, 401, 403, 404)
- ✔️ Mensagens de erro descritivas e úteis
- ✔️ Validação de integridade de dados

### 5️⃣ Paginação e Filtros

- 📄 Paginação de resultados (`?pagina=1`)
- 🔍 Filtro por nome do veículo (`?nome=civic`)
- 🏷️ Filtro por marca (`?marca=honda`)
- 🔗 Combinação de múltiplos filtros

**Exemplo:**

```
GET /veiculos?pagina=2&marca=honda&nome=civic
```

### 6️⃣ Documentação Swagger

- 📖 Interface interativa para testar endpoints
- 📋 Documentação automática dos schemas
- 💡 Exemplos de requisições e respostas
- 🔐 Suporte para autenticação Bearer Token

---

## 📁 Estrutura do Projeto

### Entidades Principais

#### **Administrador**

Representa os usuários do sistema que podem autenticar e gerenciar veículos.

```csharp
public class Administrador
{
    public int Id { get; set; }              // Chave primária auto-incremento
    public string Email { get; set; }        // Email único (até 255 chars)
    public string Senha { get; set; }        // Senha (até 50 chars) ⚠️ Em produção: usar hash!
    public string Perfil { get; set; }       // "Adm" ou "Editor" (até 10 chars)
}
```

**Observação de Segurança:** Em ambiente de produção, senhas devem ser armazenadas com hash (bcrypt, PBKDF2) e nunca em texto plano!

#### **Veículo**

Representa os veículos cadastrados no sistema.

```csharp
public class Veiculo
{
    public int Id { get; set; }              // Chave primária auto-incremento
    public string Nome { get; set; }         // Nome/modelo do veículo
    public string Marca { get; set; }        // Fabricante
    public int Ano { get; set; }             // Ano de fabricação
}
```

### DTOs (Data Transfer Objects)

#### **LoginDTO**

Usado exclusivamente para receber credenciais no endpoint de login.

```csharp
public class LoginDTO
{
    public string Email { get; set; }
    public string Senha { get; set; }
}
```

**Por que usar DTOs?**

- ✅ Separa modelo de domínio da camada de apresentação
- ✅ Evita expor propriedades sensíveis (como Id)
- ✅ Facilita validações específicas de entrada
- ✅ Permite evoluir API sem afetar banco de dados

---

## 📋 Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- **.NET SDK 8.0+** ([Download](https://dotnet.microsoft.com/download))
- **MySQL Server 8.0+** em execução
- **Git** (para clonar o repositório)
- **VS Code** ou **Visual Studio 2022** (opcional, mas recomendado)
- **Postman** ou navegador (para testar a API)

**Verificar instalações:**

```powershell
dotnet --version          # Deve retornar 8.0.x ou superior
mysql --version           # Verificar MySQL instalado
git --version             # Verificar Git instalado
```

---

## ⚙️ Configuração

### 1. Clonar o Repositório

```powershell
git clone https://github.com/diogocoding/AspNetMinimalsApi.git
cd AspNetMinimalsApi
```

### 2. Configurar Connection String

Edite o arquivo `minimal-api/appsettings.json` e ajuste a conexão do MySQL:

```json
{
  "ConnectionStrings": {
    "mysql": "server=localhost;database=lab_minimal;user=root;password=SUA_SENHA"
  }
}
```

**Parâmetros:**

- `server`: Endereço do MySQL (localhost para local)
- `database`: Nome do banco de dados (será criado automaticamente)
- `user`: Usuário MySQL com permissões de criação
- `password`: Senha do MySQL

### 3. Configurar JWT

No mesmo arquivo `appsettings.json`, configure as chaves JWT:

```json
{
  "Jwt": {
    "Key": "chave_secreta_muito_forte_com_pelo_menos_32_caracteres",
    "Issuer": "minimal-api",
    "Audience": "minimal-api-client",
    "ExpiresMinutes": 120
  }
}
```

**Parâmetros:**

- `Key`: Chave secreta para assinar tokens (⚠️ mínimo 32 caracteres recomendado)
- `Issuer`: Emissor do token (identifica a API)
- `Audience`: Público-alvo do token (identifica o consumidor)
- `ExpiresMinutes`: Tempo de validade do token em minutos

⚠️ **IMPORTANTE:** Em produção, NUNCA commite a chave secreta no repositório. Use:

- Azure Key Vault
- AWS Secrets Manager
- Variáveis de ambiente
- User Secrets (.NET)

### 4. Criar e Aplicar Migrations

Se o banco ainda não existir, crie-o e aplique as migrations:

```powershell
# Instalar ferramenta EF CLI (apenas uma vez)
dotnet tool install --global dotnet-ef

# Verificar instalação
dotnet ef --version

# Aplicar migrations (cria as tabelas)
cd AspNetMinimalsApi
dotnet ef database update --project .\minimal-api\minimal-api.csproj
```

Isso criará:

- ✅ Banco de dados `lab_minimal`
- ✅ Tabela `Administradores` com usuário padrão
- ✅ Tabela `Veiculos` (vazia inicialmente)
- ✅ Tabela `__EFMigrationsHistory` (controle de versão do schema)

**Usuário padrão criado automaticamente:**

- 📧 Email: `administrador@teste.com`
- 🔑 Senha: `123456`
- 👤 Perfil: `Adm`

---

## 🚀 Executar a API

### Opção 1: Comando direto

```powershell
cd AspNetMinimalsApi
dotnet run --project .\minimal-api\minimal-api.csproj
```

### Opção 2: Script automatizado (recomendado)

```powershell
cd AspNetMinimalsApi
powershell -ExecutionPolicy Bypass -File .\scripts\run-dev.ps1
```

O script automaticamente:

1. ⏹️ Para processos dotnet anteriores
2. 🧹 Limpa `bin/` e `obj/`
3. 📦 Restaura pacotes NuGet
4. 🔨 Compila o projeto
5. 🚀 Inicia a API em primeiro plano

**A API estará disponível em:**

- **HTTP**: `http://localhost:5162`
- **Swagger UI**: `http://localhost:5162/swagger`

**Para parar a API:** Pressione `Ctrl+C` no terminal.

**Saída esperada:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5162
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## 📖 Como Usar

### Passo 1: Obter Token JWT 🔐

Faça login para receber o token de autenticação:

**Endpoint:** `POST /auth/login`

**Request Body (JSON):**

```json
{
  "email": "administrador@teste.com",
  "senha": "123456"
}
```

**Resposta de Sucesso (200 OK):**

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5pc3RyYWRvckB0ZXN0ZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG0iLCJleHAiOjE3MzE2ODYxNTcsImlzcyI6Im1pbmltYWwtYXBpIiwiYXVkIjoibWluaW1hbC1hcGktY2xpZW50In0.xXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxX",
  "expires": "2025-11-15T14:22:37Z",
  "role": "Adm"
}
```

**Resposta de Erro (401 Unauthorized):**

- Email ou senha incorretos
- Nenhum corpo de resposta

### Passo 2: Usar o Token 🎫

Em todas as requisições protegidas, inclua o header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### 🟦 No Swagger:

1. Abra `http://localhost:5162/swagger`
2. Clique no botão **"Authorize"** (cadeado verde no topo)
3. Digite: `Bearer <seu_token>` (⚠️ com espaço entre Bearer e o token)
4. Clique em **"Authorize"**
5. Clique em **"Close"**
6. ✅ Agora você pode testar endpoints protegidos

#### 📮 No Postman:

1. Vá para a aba **"Authorization"**
2. Selecione **"Bearer Token"** no dropdown
3. Cole o token no campo **"Token"**
4. ✅ Todas as requisições usarão automaticamente o token

#### 💻 Com cURL:

```bash
curl -H "Authorization: Bearer SEU_TOKEN_AQUI" http://localhost:5162/veiculos
```

---

## 🔌 Endpoints da API

### 🏠 Home

```http
GET /
```

Retorna informações básicas da API (não requer autenticação).

**Resposta (200 OK):**

```json
{
  "name": "AspNetMinimalApi",
  "version": "v1"
}
```

---

### 🔐 Autenticação

#### Login

```http
POST /auth/login
Content-Type: application/json
```

Autentica usuário e retorna token JWT.

**Request Body:**

```json
{
  "email": "administrador@teste.com",
  "senha": "123456"
}
```

**Respostas:**

- `200 OK`: Token gerado com sucesso
- `401 Unauthorized`: Credenciais inválidas

---

### 🚗 Veículos (CRUD)

#### Listar Veículos

```http
GET /veiculos?pagina=1&nome=civic&marca=honda
Authorization: Bearer {token}
```

Lista todos os veículos com filtros opcionais e paginação.

**Query Parameters:**

- `pagina` (opcional): Número da página (default: 1)
- `nome` (opcional): Filtrar por nome do veículo (case-insensitive)
- `marca` (opcional): Filtrar por marca (case-insensitive)

**Autenticação:** ✅ Requerida (qualquer usuário autenticado)

**Resposta (200 OK):**

```json
[
  {
    "id": 1,
    "nome": "Civic",
    "marca": "Honda",
    "ano": 2024
  },
  {
    "id": 2,
    "nome": "Corolla",
    "marca": "Toyota",
    "ano": 2023
  }
]
```

---

#### Buscar Veículo por ID

```http
GET /veiculos/{id}
Authorization: Bearer {token}
```

Retorna um veículo específico.

**Parâmetros de Rota:**

- `id` (int): ID do veículo

**Autenticação:** ✅ Requerida

**Respostas:**

- `200 OK`: Veículo encontrado
- `404 Not Found`: Veículo não existe
- `401 Unauthorized`: Token inválido ou ausente

**Exemplo (200 OK):**

```json
{
  "id": 1,
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2024
}
```

---

#### Criar Veículo

```http
POST /veiculos
Authorization: Bearer {token}
Content-Type: application/json
```

Cadastra um novo veículo.

**Autenticação:** ✅ Requerida (Adm ou Editor)

**Request Body:**

```json
{
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2024
}
```

**Respostas:**

- `201 Created`: Veículo criado com sucesso
- `400 Bad Request`: Dados inválidos (com detalhes dos erros)
- `401 Unauthorized`: Token inválido ou ausente
- `403 Forbidden`: Usuário sem permissão

**Exemplo de Erro (400 Bad Request):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": ["Nome é obrigatório."],
    "Ano": ["Ano deve estar entre 1900 e 2026."]
  }
}
```

---

#### Atualizar Veículo

```http
PUT /veiculos/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

Atualiza dados de um veículo existente.

**Autenticação:** ✅ Requerida (Adm ou Editor)

**Request Body:**

```json
{
  "id": 1,
  "nome": "Civic 2025 Atualizado",
  "marca": "Honda",
  "ano": 2025
}
```

**Respostas:**

- `204 No Content`: Atualizado com sucesso
- `400 Bad Request`: ID da URL não coincide com ID do body
- `403 Forbidden`: Usuário sem permissão
- `401 Unauthorized`: Token inválido

---

#### Deletar Veículo

```http
DELETE /veiculos/{id}
Authorization: Bearer {token}
```

Remove um veículo do sistema.

**Autenticação:** ✅ Requerida (apenas Adm)

**Parâmetros de Rota:**

- `id` (int): ID do veículo a ser deletado

**Respostas:**

- `204 No Content`: Deletado com sucesso
- `404 Not Found`: Veículo não encontrado
- `403 Forbidden`: Apenas administradores podem deletar
- `401 Unauthorized`: Token inválido

---

## ⚖️ Regras de Negócio

### Validações de Veículo

Todas as validações são aplicadas nos endpoints `POST /veiculos` e `PUT /veiculos/{id}`.

| Campo     | Regra                                             | Mensagem de Erro                             |
| --------- | ------------------------------------------------- | -------------------------------------------- |
| **Nome**  | Obrigatório, não pode ser vazio ou apenas espaços | "Nome é obrigatório."                        |
| **Marca** | Obrigatória, não pode ser vazia ou apenas espaços | "Marca é obrigatória."                       |
| **Ano**   | Deve ser >= 1900 e <= (ano atual + 1)             | "Ano deve estar entre 1900 e {ano_atual+1}." |

**Exemplo de validação de Ano:**

- Em 2025, o ano máximo permitido é 2026 (permite cadastro de modelos do próximo ano)
- Ano mínimo: 1900 (carros históricos)

### Políticas de Autorização

| Endpoint                | Perfil Necessário    | Descrição                                          |
| ----------------------- | -------------------- | -------------------------------------------------- |
| `GET /veiculos`         | Qualquer autenticado | Qualquer usuário logado pode listar                |
| `GET /veiculos/{id}`    | Qualquer autenticado | Qualquer usuário logado pode visualizar            |
| `POST /veiculos`        | Adm ou Editor        | Criar veículos requer permissão de edição          |
| `PUT /veiculos/{id}`    | Adm ou Editor        | Atualizar veículos requer permissão de edição      |
| `DELETE /veiculos/{id}` | **Apenas Adm**       | Deletar é operação crítica, apenas administradores |

**Hierarquia de Perfis:**

- **Adm** > **Editor** > **Autenticado**

---

## 🔒 Segurança

### Implementações de Segurança Aplicadas

#### 1. **Autenticação JWT**

- ✅ Tokens assinados com **HMAC-SHA256**
- ✅ Validação de **issuer** (emissor do token)
- ✅ Validação de **audience** (público-alvo)
- ✅ Validação de **assinatura** (integridade)
- ✅ Validação de **expiração** (lifetime)
- ✅ Middleware `UseAuthentication()` automático

**Código de configuração:**

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            // ...
        };
    });
```

#### 2. **Autorização por Roles**

- ✅ Políticas customizadas (`AdmOnly`, `AdmOrEditor`)
- ✅ Validação automática pelo middleware `UseAuthorization()`
- ✅ Resposta **403 Forbidden** para usuários sem permissão
- ✅ Claims de role validadas a cada requisição

**Exemplo de política:**

```csharp
options.AddPolicy("AdmOnly", p => p.RequireRole("Adm"));
```

#### 3. **Validação de Entrada**

- ✅ Validação de dados no endpoint (antes de chamar serviços)
- ✅ Mensagens de erro descritivas e estruturadas
- ✅ Prevenção de dados inconsistentes no banco
- ✅ Retorno de **400 Bad Request** com detalhes

#### 4. **HTTPS (Recomendado em Produção)**

- ⚠️ Tokens JWT devem trafegar **apenas via HTTPS**
- ⚠️ Configurar certificado SSL/TLS válido
- ⚠️ Habilitar `app.UseHttpsRedirection()` em produção

### Melhores Práticas Aplicadas

| Prática                       | Status | Descrição                                            |
| ----------------------------- | ------ | ---------------------------------------------------- |
| Senhas não retornadas em JSON | ✅     | Entidade `Administrador` nunca é exposta diretamente |
| Tokens com expiração          | ✅     | Default: 120 minutos configurável                    |
| Claims mínimas                | ✅     | Apenas email e role no token                         |
| Validação de autorização      | ✅     | Antes de processar requisições                       |
| Separação DTOs/Entidades      | ✅     | `LoginDTO` separado de `Administrador`               |
| Dependency Injection          | ✅     | Todas as dependências injetadas via DI               |

### ⚠️ Avisos de Segurança

**NUNCA em produção:**

- ❌ Comitar senhas ou chaves secretas no código
- ❌ Usar senhas fracas (como "123456" do seed)
- ❌ Expor stack traces de erros ao usuário
- ❌ Permitir HTTP sem HTTPS
- ❌ Armazenar senhas em texto plano

**Recomendações para produção:**

- ✅ Use **Azure Key Vault** ou **AWS Secrets Manager** para secrets
- ✅ Implemente **rate limiting** (ex: AspNetCoreRateLimit)
- ✅ Adicione **logs de auditoria** (quem fez o quê e quando)
- ✅ **Hash de senhas** com bcrypt, Argon2 ou PBKDF2
- ✅ Configure **CORS** adequadamente
- ✅ Implemente **refresh tokens** para sessões longas
- ✅ Use **User Secrets** para desenvolvimento local

**Exemplo de User Secrets (.NET):**

```powershell
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "chave_secreta_forte"
```

---

## 📝 Exemplos de Uso

### Exemplo Completo com cURL

#### 1️⃣ Fazer Login

```bash
curl -X POST http://localhost:5162/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"administrador@teste.com","senha":"123456"}'
```

**Resposta:**

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2025-11-15T14:22:37Z",
  "role": "Adm"
}
```

#### 2️⃣ Criar Veículo

```bash
# Substitua TOKEN pelo access_token recebido
curl -X POST http://localhost:5162/veiculos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{"nome":"Civic","marca":"Honda","ano":2024}'
```

#### 3️⃣ Listar Veículos

```bash
curl -X GET http://localhost:5162/veiculos \
  -H "Authorization: Bearer TOKEN"
```

#### 4️⃣ Buscar Veículo Específico

```bash
curl -X GET http://localhost:5162/veiculos/1 \
  -H "Authorization: Bearer TOKEN"
```

#### 5️⃣ Atualizar Veículo

```bash
curl -X PUT http://localhost:5162/veiculos/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{"id":1,"nome":"Civic 2025","marca":"Honda","ano":2025}'
```

#### 6️⃣ Deletar Veículo

```bash
# Apenas administradores podem deletar
curl -X DELETE http://localhost:5162/veiculos/1 \
  -H "Authorization: Bearer TOKEN"
```

#### 7️⃣ Testar Segurança (sem token)

```bash
# Deve retornar 401 Unauthorized
curl -X GET http://localhost:5162/veiculos
```

---

### Exemplo com PowerShell

```powershell
# 1. Login e obter token
$loginBody = @{
    email = "administrador@teste.com"
    senha = "123456"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5162/auth/login" `
    -Method POST `
    -Body $loginBody `
    -ContentType "application/json"

$token = $response.access_token
Write-Host "Token obtido: $($token.Substring(0,20))..."

# 2. Criar veículo
$headers = @{ Authorization = "Bearer $token" }
$veiculoBody = @{
    nome = "Civic"
    marca = "Honda"
    ano = 2024
} | ConvertTo-Json

$criado = Invoke-RestMethod -Uri "http://localhost:5162/veiculos" `
    -Method POST `
    -Headers $headers `
    -Body $veiculoBody `
    -ContentType "application/json"

Write-Host "Veículo criado com ID: $($criado.id)"

# 3. Listar veículos
$veiculos = Invoke-RestMethod -Uri "http://localhost:5162/veiculos" `
    -Method GET `
    -Headers $headers

$veiculos | Format-Table
```

---

## 🧪 Testando com Postman

### 1. Importe a Collection

Crie uma coleção "AspNetMinimalApi" e adicione estas variáveis de ambiente:

| Variável  | Valor                             |
| --------- | --------------------------------- |
| `baseUrl` | `http://localhost:5162`           |
| `token`   | (será preenchida automaticamente) |

### 2. Configure Pre-request Script (Login Automático)

Na aba "Pre-request Script" da coleção, adicione:

```javascript
// Auto-login antes de cada requisição
pm.sendRequest(
  {
    url: pm.environment.get("baseUrl") + "/auth/login",
    method: "POST",
    header: { "Content-Type": "application/json" },
    body: {
      mode: "raw",
      raw: JSON.stringify({
        email: "administrador@teste.com",
        senha: "123456",
      }),
    },
  },
  function (err, res) {
    if (!err && res.code === 200) {
      pm.environment.set("token", res.json().access_token);
      console.log("✅ Token atualizado automaticamente");
    } else {
      console.error("❌ Erro ao obter token:", err || res.text());
    }
  }
);
```

### 3. Configure Authorization

Em todas as requisições (exceto `/auth/login`):

- **Type**: Bearer Token
- **Token**: `{{token}}`

### 4. Crie as Requisições

| Nome              | Método | URL                      | Body                                                   |
| ----------------- | ------ | ------------------------ | ------------------------------------------------------ |
| Login             | POST   | `{{baseUrl}}/auth/login` | `{"email":"administrador@teste.com","senha":"123456"}` |
| Listar Veículos   | GET    | `{{baseUrl}}/veiculos`   | -                                                      |
| Criar Veículo     | POST   | `{{baseUrl}}/veiculos`   | `{"nome":"Civic","marca":"Honda","ano":2024}`          |
| Buscar Veículo    | GET    | `{{baseUrl}}/veiculos/1` | -                                                      |
| Atualizar Veículo | PUT    | `{{baseUrl}}/veiculos/1` | `{"id":1,"nome":"Civic","marca":"Honda","ano":2025}`   |
| Deletar Veículo   | DELETE | `{{baseUrl}}/veiculos/1` | -                                                      |

---

## 🐛 Troubleshooting (Solução de Problemas)

### Problema: "Cannot connect to MySQL"

**Erro:**

```
Unable to connect to any of the specified MySQL hosts.
```

**Solução:**

1. Verifique se MySQL está rodando: `mysql -u root -p`
2. Confirme as credenciais em `appsettings.json`
3. Teste a conexão: `mysql -h localhost -u root -p`

---

### Problema: "401 Unauthorized" ao acessar endpoints

**Causas possíveis:**

- Token expirado
- Token não enviado no header
- Formato incorreto do header

**Solução:**

1. Faça login novamente: `POST /auth/login`
2. Verifique o header: `Authorization: Bearer TOKEN` (com espaço!)
3. Confirme que o token não expirou (default: 120 minutos)

---

### Problema: "403 Forbidden"

**Erro:** Usuário autenticado, mas sem permissão.

**Solução:**

- Verifique se o usuário tem o perfil correto (Adm para deletar, Adm ou Editor para criar/atualizar)
- Confirme as claims do token: decodifique em [jwt.io](https://jwt.io)

---

### Problema: Arquivo `minimal-api.exe` ou `apphost.exe` em uso

**Erro ao compilar:**

```
MSB3027: Não foi possível copiar "obj\Debug\net8.0\apphost.exe" para "bin\Debug\net8.0\apphost.exe"
```

**Solução:**

```powershell
# Parar todos os processos dotnet
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# Limpar bin e obj
Remove-Item .\minimal-api\bin -Recurse -Force
Remove-Item .\minimal-api\obj -Recurse -Force

# Recompilar
dotnet build .\AspNetMinimalsApi.sln
```

---

### Problema: Migration já aplicada

**Erro:**

```
The migration '..._VeiculosMigration' has already been applied to the database.
```

**Solução:**

- Isso é normal! As migrations já foram aplicadas.
- Para reverter: `dotnet ef database update 0 --project .\minimal-api\minimal-api.csproj`
- Para reaplicar: `dotnet ef database update --project .\minimal-api\minimal-api.csproj`

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Faça um **fork** do projeto
2. Crie uma **branch** para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. **Commit** suas mudanças (`git commit -m 'feat: Adiciona nova funcionalidade'`)
4. Faça **push** para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um **Pull Request** descrevendo suas mudanças

**Padrões de commit (Conventional Commits):**

- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Alterações na documentação
- `refactor`: Refatoração sem mudança de comportamento
- `test`: Adição ou correção de testes

---

## 📄 Licença

Este projeto é para **fins educacionais** e não possui licença comercial.

---

## 👨‍💻 Autor

Desenvolvido como projeto de aprendizado em **ASP.NET Core Minimal APIs**.

**Repositório:** [diogocoding/AspNetMinimalsApi](https://github.com/diogocoding/AspNetMinimalsApi)

---

## 📚 Recursos e Referências

### Documentação Oficial

- [.NET 8 - What's New](https://learn.microsoft.com/pt-br/dotnet/core/whats-new/dotnet-8)
- [Minimal APIs Overview](https://learn.microsoft.com/pt-br/aspnet/core/fundamentals/minimal-apis)
- [Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)
- [ASP.NET Core Security](https://learn.microsoft.com/pt-br/aspnet/core/security/)

### Ferramentas e Recursos

- [JWT.io](https://jwt.io/) - Debug e validação de tokens JWT
- [Swagger/OpenAPI](https://swagger.io/) - Documentação de APIs
- [Postman](https://www.postman.com/) - Teste de APIs
- [MySQL Workbench](https://www.mysql.com/products/workbench/) - GUI para MySQL

### Bibliotecas Utilizadas

- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) - Provider MySQL para EF Core
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Swagger para ASP.NET Core
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/) - Autenticação JWT

### Artigos e Tutoriais

- [Securing APIs with JWT](https://jwt.io/introduction)
- [Entity Framework Core Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
- [Minimal API Best Practices](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)

---

**✨ Projeto pronto para desenvolvimento e aprendizado!**

**🚀 Para produção:**

- Configure HTTPS com certificado válido
- Implemente hash de senhas (bcrypt/Argon2)
- Use secrets managers (Azure Key Vault/AWS Secrets)
- Adicione rate limiting e logs de auditoria
- Configure CORS adequadamente
- Implemente monitoramento (Application Insights)
