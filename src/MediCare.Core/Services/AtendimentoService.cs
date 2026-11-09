using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Casos de uso do médico: UC10 (gerenciar agenda) e UC15 (consultar histórico do paciente).
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

    private HorarioDisponivel Horario(Consulta consulta) => dados.Horarios.ObterPorId(consulta.HorarioId)!;
}
