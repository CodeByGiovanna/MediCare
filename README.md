# MediCare

Sistema de agendamento de consultas médicas em C# (.NET 8), com os dados
gravados em arquivos CSV.

- `MediCare.Core`: classes do modelo, leitura e escrita dos CSV e serviços
  com as regras de cada caso de uso.
- `MediCare.CsvApp`: aplicação de console com login e um menu para cada
  perfil (paciente, recepcionista, médico e administrador).

## Como executar

```
dotnet run --project src/MediCare.CsvApp -- dados
```

O argumento é a pasta dos CSV (padrão: `./dados`).

## Usuários de exemplo

Todos usam a senha `senha123`.

| Perfil        | E-mail                     |
|---------------|----------------------------|
| Paciente      | ana.souza@email.com        |
| Paciente      | bruno.lima@email.com       |
| Recepcionista | recepcao@medicare.com      |
| Médico        | carla.mendes@medicare.com  |
| Administrador | admin@medicare.com         |

A paciente `camila.torres@gmail.com` usa login pelo Google e não tem senha
na plataforma.
