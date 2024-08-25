# DIO - Desafio - Trabalhando com ASP.NET Minimals APIs

[www.dio.me](https://www.dio.me/)

## Descrição

A proposta do projeto é criar uma Minimal API para gerenciar veículos.

É possível fazer login para obter um JSON Web Token (JWT) e acessar os demais endpoints. A API possui 2 perfis de usuários administradores:

- **Adm**: possui acesso total
- **Editor**: pode apenas criar e listar veículos

As funcionalidades são:

- Login
- Criar administrador
- Listar administradores
- Buscar administrador por Id
- Criar veículo
- Listar veículos
- Buscar veículo por Id
- Atualizar veículo
- Deletar veículo

## Guia de uso

A API utiliza banco de dados MySQL. A string de conexão pode ser alterada no arquivo `appsettings.json`.

É necessário instalar as ferramentas de CLI do Entity Framework Core através do comando:

```ps
dotnet tool install --global dotnet-ef
```

Com as ferramentas instaladas, o comando para executar as migrations e criar o banco com as tabelas é:

```ps
dotnet ef database update
```

Um administrador com perfil **Adm** é adicionado com as migrations:

- **Email**: administrador@teste.com
- **Senha**: 123456
