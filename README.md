# FIAP Cloud Games API

API REST para a plataforma FIAP Cloud Games, desenvolvida em .NET 8.

## Funcionalidades

- Cadastro e autenticação de usuários
- Gerenciamento de jogos (apenas administradores)
- Biblioteca de jogos por usuário
- Autenticação via JWT
- Documentação com Swagger

## Requisitos

- .NET 8 SDK
- Visual Studio 2022 ou VS Code

## Configuração

1. Clone o repositório
2. Abra o projeto na sua IDE preferida
3. Execute o comando `dotnet restore` para restaurar as dependências
4. Execute o comando `dotnet build` para compilar o projeto
5. Execute o comando `dotnet run` para iniciar a API

## Endpoints

### Usuários

- `POST /api/users` - Cadastro de usuário
- `POST /api/auth/login` - Autenticação

### Jogos

- `POST /api/games` - Cadastro de jogo (apenas admin)
- `GET /api/games` - Lista todos os jogos

### Biblioteca de Jogos

- `POST /api/users/games` - Adiciona jogo à biblioteca
- `GET /api/users/games` - Lista jogos da biblioteca

## Documentação

A documentação da API está disponível através do Swagger UI quando a aplicação está em execução:

```
https://localhost:5001/swagger
```

## Testes

Para executar os testes unitários:

```bash
dotnet test
```

## Tecnologias Utilizadas

- .NET 8
- Entity Framework Core (InMemory)
- JWT Authentication
- Swagger/OpenAPI
- xUnit (testes)
