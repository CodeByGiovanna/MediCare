using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

/// <summary>
/// Reúne os repositórios de todas as entidades. A forma de persistência (CSV ou EF Core)
/// é escolhida por quem cria o objeto; os serviços não dependem dela.
/// </summary>
public class MediCareDados
{
    public required IRepositorio<Especialidade> Especialidades { get; init; }
    public required IRepositorio<Paciente> Pacientes { get; init; }
    public required IRepositorio<Medico> Medicos { get; init; }
    public required IRepositorio<Recepcionista> Recepcionistas { get; init; }
    public required IRepositorio<Administrador> Administradores { get; init; }
    public required IRepositorio<LocalAtendimento> Locais { get; init; }
    public required IRepositorio<HorarioDisponivel> Horarios { get; init; }
    public required IRepositorio<Consulta> Consultas { get; init; }
    public required IRepositorio<Notificacao> Notificacoes { get; init; }

    public static MediCareDados DeArquivosCsv(string pasta) => new()
    {
        Especialidades = new EspecialidadeRepository(Path.Combine(pasta, "especialidades.csv")),
        Pacientes = new PacienteRepository(Path.Combine(pasta, "pacientes.csv")),
        Medicos = new MedicoRepository(Path.Combine(pasta, "medicos.csv")),
        Recepcionistas = new RecepcionistaRepository(Path.Combine(pasta, "recepcionistas.csv")),
        Administradores = new AdministradorRepository(Path.Combine(pasta, "administradores.csv")),
        Locais = new LocalAtendimentoRepository(Path.Combine(pasta, "locais_atendimento.csv")),
        Horarios = new HorarioRepository(Path.Combine(pasta, "horarios.csv")),
        Consultas = new ConsultaRepository(Path.Combine(pasta, "consultas.csv")),
        Notificacoes = new NotificacaoRepository(Path.Combine(pasta, "notificacoes.csv"))
    };

    public IEnumerable<Usuario> TodosOsUsuarios() =>
        Pacientes.ListarTodos().Cast<Usuario>()
            .Concat(Medicos.ListarTodos())
            .Concat(Recepcionistas.ListarTodos())
            .Concat(Administradores.ListarTodos());

    public void SalvarUsuarios()
    {
        Pacientes.Salvar();
        Medicos.Salvar();
        Recepcionistas.Salvar();
        Administradores.Salvar();
    }
}
