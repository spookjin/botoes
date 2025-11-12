# Lead Manager

Aplicação de gerenciamento de leads composta por uma API .NET 6 e uma SPA em React.

## Estrutura do projeto

```
backend/
 ├── LeadManager.sln
 ├── LeadManager.Api/        # API ASP.NET Core 6
 └── LeadManager.Tests/      # Testes unitários xUnit
css/                         # Estilos da SPA
js/                          # Código React (Babel standalone)
index.html                   # Ponto de entrada da aplicação
```

## Pré-requisitos

- [.NET SDK 6.0](https://dotnet.microsoft.com/en-us/download)
- SQL Server (pode ser LocalDB ou uma instância Docker acessível)
- Node.js **não** é necessário: a SPA utiliza React via CDN

## Configurando o banco de dados

1. Atualize a connection string em `backend/LeadManager.Api/appsettings.json` caso não utilize `LocalDB`.
2. Execute as migrações (ou utilize `EnsureCreated`) para criar o schema:

```bash
cd backend
 dotnet tool restore
 dotnet ef database update --project LeadManager.Api/LeadManager.Api.csproj
```

> O projeto chama `EnsureCreated()` no startup para facilitar os testes locais. Em produção recomenda-se utilizar migrações.

## Executando a API

```bash
cd backend
 dotnet restore
 dotnet run --project LeadManager.Api/LeadManager.Api.csproj
```

A API será iniciada (por padrão) em `https://localhost:7088` e `http://localhost:5088`.

### Endpoints principais

- `GET /api/leads?status=Invited|Accepted` – lista leads por status
- `POST /api/leads/{id}/accept` – aceita o lead, aplica desconto de 10% quando `price > 500` e registra notificação
- `POST /api/leads/{id}/decline` – marca lead como recusado

As notificações de e-mail são simuladas em `./notifications/email-log.txt`.

## Executando os testes unitários

```bash
cd backend
 dotnet test
```

Os testes cobrem a lógica de aceitação/recusa de leads, incluindo a regra de desconto e a chamada do serviço de e-mail.

## Executando a SPA

1. Certifique-se de que a API está em execução em `http://localhost:5088` (ou ajuste a constante `API_BASE_URL` em `js/app.js`).
2. Abra `index.html` diretamente no navegador ou sirva a pasta via qualquer servidor estático.

A SPA possui duas abas:

- **Invited**: lista os leads novos com ações de *Accept* e *Decline*.
- **Accepted**: mostra os leads aceitos, incluindo telefone e e-mail de contato.

As ações consomem a API e atualizam as listas em tempo real.
