namespace MediCare.Core.Models;

public class Consulta : IEntidade
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int HorarioId { get; private set; }
    public StatusConsulta Status { get; private set; } = StatusConsulta.Agendada;
    public DateTime DataAgendamento { get; private set; }

    // Usado pelo EF Core ao materializar a entidade.
    private Consulta() { }

    public Consulta(int pacienteId, int horarioId)
    {
        PacienteId = pacienteId;
        HorarioId = horarioId;
        DataAgendamento = DateTime.Now;
    }

    internal Consulta(int id, int pacienteId, int horarioId, StatusConsulta status, DateTime dataAgendamento)
    {
        Id = id;
        PacienteId = pacienteId;
        HorarioId = horarioId;
        Status = status;
        DataAgendamento = dataAgendamento;
    }

    public void Cancelar()
    {
        GarantirAgendada();
        Status = StatusConsulta.Cancelada;
    }

    public void Reagendar(int novoHorarioId)
    {
        GarantirAgendada();
        HorarioId = novoHorarioId;
    }

    public void Concluir()
    {
        GarantirAgendada();
        Status = StatusConsulta.Realizada;
    }

    private void GarantirAgendada()
    {
        if (Status != StatusConsulta.Agendada)
            throw new InvalidOperationException(
                $"A consulta está {Status.ToString().ToLower()} e não pode ser alterada.");
    }
}
