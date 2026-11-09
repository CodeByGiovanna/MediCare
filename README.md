# MediCare

Plataforma de agendamento e gerenciamento de consultas médicas, desenvolvida no
Projeto de Bloco de Desenvolvimento Back-End (INFNET).

Pacientes pesquisam médicos por especialidade, consultam horários e agendam
consultas presenciais ou por teleconsulta. A recepção agenda em nome dos
pacientes, os médicos gerenciam disponibilidade e agenda, e o administrador
cuida dos usuários, dos agendamentos e do envio de notificações.

## Tecnologias

- C# / .NET 8
- Persistência em arquivos CSV (RFC 4180)
- Aplicação de console (a interface Razor Pages está prevista para a próxima etapa)

## Estrutura

```
dados/                      arquivos CSV de exemplo
docs/                       documentação de cada etapa (TP1 a TP4)
src/MediCare.Core/
  Models/                   entidades e enumerações
  Csv/                      leitura e escrita de CSV
  Repositories/             um repositório por arquivo CSV
  Services/                 regras dos casos de uso
src/MediCare.CsvApp/        aplicação de console com menu por perfil
```

## Como executar

Requer o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
dotnet run --project src/MediCare.CsvApp -- dados
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
