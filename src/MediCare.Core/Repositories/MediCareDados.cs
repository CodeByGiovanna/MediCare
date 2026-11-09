using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

/// <summary>
/// Reúne os repositórios de todas as entidades em uma pasta de dados.
/// </summary>
public class MediCareDados(string pasta)
{
    public EspecialidadeRepository Especialidades { get; } = new(Path.Combine(pasta, "especialidades.csv"));
    public PacienteRepository Pacientes { get; } = new(Path.Combine(pasta, "pacientes.csv"));
    public MedicoRepository Medicos { get; } = new(Path.Combine(pasta, "medicos.csv"));
    public RecepcionistaRepository Recepcionistas { get; } = new(Path.Combine(pasta, "recepcionistas.csv"));
    public AdministradorRepository Administradores { get; } = new(Path.Combine(pasta, "administradores.csv"));
    public LocalAtendimentoRepository Locais { get; } = new(Path.Combine(pasta, "locais_atendimento.csv"));
    public HorarioRepository Horarios { get; } = new(Path.Combine(pasta, "horarios.csv"));
    public ConsultaRepository Consultas { get; } = new(Path.Combine(pasta, "consultas.csv"));
    public NotificacaoRepository Notificacoes { get; } = new(Path.Combine(pasta, "notificacoes.csv"));

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
