using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Casos de uso de atendimento: UC10 (gerenciar agenda), UC11 (realizar teleconsulta)
/// e UC15 (consultar histórico do paciente).
/// </summary>
public class AtendimentoService(MediCareDados dados)
{
    public IEnumerable<Consulta> Agenda(int medicoId) =>
        dados.Consultas.ListarTodos()
            .Where(c => c.Status == StatusConsulta.Agendada && Horario(c).MedicoId == medicoId)
            .OrderBy(c => Horario(c).Data).ThenBy(c => Horario(c).HoraInicio);

    public void ConcluirConsulta(int medicoId, int consultaId)
    {
        var consulta = dados.Consultas.ObterPorId(consultaId);
        if (consulta is null || Horario(consulta).MedicoId != medicoId)
            throw new InvalidOperationException("Consulta não encontrada na sua agenda.");

        consulta.Concluir();
        dados.Consultas.Salvar();
    }

    public IEnumerable<Consulta> HistoricoDoPaciente(int medicoId, int pacienteId)
    {
        var consultasDoPaciente = dados.Consultas.ListarTodos()
            .Where(c => c.PacienteId == pacienteId)
            .ToList();

        if (!consultasDoPaciente.Any(c => Horario(c).MedicoId == medicoId))
            throw new InvalidOperationException(
                "Só é possível ver o histórico de pacientes atendidos por você.");

        return consultasDoPaciente
            .Where(c => c.Status == StatusConsulta.Realizada)
            .OrderByDescending(c => Horario(c).Data).ThenByDescending(c => Horario(c).HoraInicio);
    }

    public Consulta AcessarTeleconsulta(Usuario usuario, int consultaId, DateTime agora)
    {
        var consulta = dados.Consultas.ObterPorId(consultaId)
            ?? throw new InvalidOperationException("Consulta não encontrada.");
        var horario = Horario(consulta);

        var participa = usuario switch
        {
            Paciente p => p.Id == consulta.PacienteId,
            Medico m => m.Id == horario.MedicoId,
            _ => false
        };
        if (!participa)
            throw new InvalidOperationException("Você não participa desta consulta.");
        if (horario.Modalidade != Modalidade.Teleconsulta)
            throw new InvalidOperationException("Esta consulta é presencial.");
        if (consulta.Status != StatusConsulta.Agendada)
            throw new InvalidOperationException("A consulta não está agendada.");

        var inicio = horario.Data.ToDateTime(horario.HoraInicio);
        var fim = horario.Data.ToDateTime(horario.HoraFim);
        if (agora < inicio || agora > fim)
            throw new InvalidOperationException(
                $"A teleconsulta só pode ser acessada no horário agendado ({inicio:dd/MM/yyyy HH:mm} a {fim:HH:mm}).");

        return consulta;
    }

    public IEnumerable<Consulta> TeleconsultasAgendadas(Usuario usuario) =>
        dados.Consultas.ListarTodos()
            .Where(c => c.Status == StatusConsulta.Agendada && Horario(c).Modalidade == Modalidade.Teleconsulta)
            .Where(c => usuario switch
            {
                Paciente p => c.PacienteId == p.Id,
                Medico m => Horario(c).MedicoId == m.Id,
                _ => false
            });

    private HorarioDisponivel Horario(Consulta consulta) => dados.Horarios.ObterPorId(consulta.HorarioId)!;
}
