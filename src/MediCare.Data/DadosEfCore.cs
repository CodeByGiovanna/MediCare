using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Data;

public static class DadosEfCore
{
    public static MediCareDados Criar(MediCareDbContext contexto) => new()
    {
        Especialidades = new EfRepositorio<Especialidade>(contexto),
        Pacientes = new EfRepositorio<Paciente>(contexto),
        Medicos = new EfRepositorio<Medico>(contexto),
        Recepcionistas = new EfRepositorio<Recepcionista>(contexto),
        Administradores = new EfRepositorio<Administrador>(contexto),
        Locais = new EfRepositorio<LocalAtendimento>(contexto),
        Horarios = new EfRepositorio<HorarioDisponivel>(contexto),
        Consultas = new EfRepositorio<Consulta>(contexto),
        Notificacoes = new EfRepositorio<Notificacao>(contexto)
    };

    /// <summary>
    /// Cria o banco, se ainda não existir, e o preenche com os dados de exemplo
    /// dos arquivos CSV usados nas etapas anteriores do projeto.
    /// </summary>
    public static void PrepararBanco(MediCareDbContext contexto, string pastaCsv)
    {
        contexto.Database.EnsureCreated();
        if (contexto.Especialidades.Any())
            return;

        var csv = MediCareDados.DeArquivosCsv(pastaCsv);
        contexto.Especialidades.AddRange(csv.Especialidades.ListarTodos());
        contexto.Pacientes.AddRange(csv.Pacientes.ListarTodos());
        contexto.Medicos.AddRange(csv.Medicos.ListarTodos());
        contexto.Recepcionistas.AddRange(csv.Recepcionistas.ListarTodos());
        contexto.Administradores.AddRange(csv.Administradores.ListarTodos());
        contexto.Locais.AddRange(csv.Locais.ListarTodos());
        contexto.Horarios.AddRange(csv.Horarios.ListarTodos());
        contexto.Consultas.AddRange(csv.Consultas.ListarTodos());
        contexto.Notificacoes.AddRange(csv.Notificacoes.ListarTodos());
        contexto.SaveChanges();
    }
}
