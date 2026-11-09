# MediCare

Plataforma de agendamento e gerenciamento de consultas médicas, desenvolvida no
Projeto de Bloco de Desenvolvimento Back-End (INFNET).

Pacientes pesquisam médicos por especialidade, consultam horários e agendam
consultas presenciais ou por teleconsulta. A recepção agenda em nome dos
pacientes, os médicos gerenciam disponibilidade e agenda, e o administrador
cuida dos usuários, dos agendamentos e do envio de notificações.

## Tecnologias

- C# / .NET 8
- ASP.NET Core Razor Pages (interface e back-end)
- EF Core com SQLite (persistência)
- xUnit (testes automatizados)

## Estrutura

```
dados/                      dados de exemplo em CSV (carregados no banco na primeira execução)
docs/                       documentação de cada etapa (TP1 a TP5)
src/MediCare.Core/          entidades, regras de negócio (serviços) e leitura/escrita de CSV
src/MediCare.Data/          persistência com EF Core (SQLite)
src/MediCare.Web/           aplicação Razor Pages
tests/MediCare.Tests/       testes automatizados
```

## Como executar

Requer o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
dotnet run --project src/MediCare.Web
```

Acesse http://localhost:5080. Na primeira execução é criado o banco
`src/MediCare.Web/medicare.db` com os dados de exemplo.

Para executar os testes:

```
dotnet test
```

## Usuários de exemplo

Todos usam a senha `senha123`.

| Perfil        | E-mail                     |
|---------------|----------------------------|
| Paciente      | ana.souza@email.com        |
| Paciente      | bruno.lima@email.com       |
| Recepcionista | recepcao@medicare.com      |
| Médico        | carla.mendes@medicare.com  |
| Administrador | admin@medicare.com         |

## Documentação

| Etapa | Conteúdo |
|-------|----------|
| [TP1](docs/TP1_Requisitos.pdf) | Descrição do sistema e requisitos |
| [TP2](docs/TP2_Casos_de_Uso.pdf) | Atores e casos de uso |
| [TP3](docs/TP3_Modelagem_de_Classes.pdf) | Classes, dicionário de dados e leitura/escrita de CSV |
| [TP4](docs/TP4_Mudanca_de_Requisitos.pdf) | Mudança de requisitos, diagramas de sequência e de comunicação |
| [TP5](docs/TP5_Entrega_Final.pdf) | Aplicação Razor Pages, persistência com EF Core e testes |
