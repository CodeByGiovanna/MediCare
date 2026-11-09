using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

/// <summary>
/// Passos de agendamento comuns ao paciente e à recepcionista (UC03 a UC07).
/// </summary>
public class FluxoAgendamento(Sistema s)
{
    public void PesquisarMedicos()
    {
        s.Exibir.Especialidades();
        var especialidadeId = Entrada.LerIntOpcional("Especialidade (Enter para todas)");
        s.Exibir.Medicos(s.Agendamento.PesquisarMedicos(especialidadeId));
    }

    public void VerHorarios()
    {
        s.Exibir.Medicos(s.Agendamento.PesquisarMedicos());
        s.Exibir.Horarios(s.Agendamento.HorariosDisponiveis(Entrada.LerInt("Médico")));
    }

    public void Agendar(Paciente paciente)
    {
        s.Exibir.Medicos(s.Agendamento.PesquisarMedicos());
        s.Exibir.Horarios(s.Agendamento.HorariosDisponiveis(Entrada.LerInt("Médico")));

        var consulta = s.Agendamento.Agendar(paciente.Id, Entrada.LerInt("Horário"));
        Console.WriteLine($"Consulta {consulta.Id} agendada. Confirmação enviada por {paciente.CanalPreferido}.");
    }

    public void ListarConsultas(Paciente paciente) =>
        s.Exibir.Consultas(s.Agendamento.ConsultasDoPaciente(paciente.Id));

    public void Cancelar(Paciente paciente)
    {
        var consulta = EscolherConsultaAgendada(paciente);
        s.Agendamento.Cancelar(consulta.Id);
        Console.WriteLine("Consulta cancelada e horário liberado.");
    }

    public void Reagendar(Paciente paciente)
    {
        var consulta = EscolherConsultaAgendada(paciente);
        var medicoId = s.Dados.Horarios.ObterPorId(consulta.HorarioId)!.MedicoId;

        s.Exibir.Horarios(s.Agendamento.HorariosDisponiveis(medicoId));
        s.Agendamento.Reagendar(consulta.Id, Entrada.LerInt("Novo horário"));
        Console.WriteLine("Consulta reagendada.");
    }

    private Consulta EscolherConsultaAgendada(Paciente paciente)
    {
        var agendadas = s.Agendamento.ConsultasDoPaciente(paciente.Id)
            .Where(c => c.Status == StatusConsulta.Agendada)
            .ToList();
        s.Exibir.Consultas(agendadas);

        var id = Entrada.LerInt("Consulta");
        return agendadas.FirstOrDefault(c => c.Id == id)
            ?? throw new InvalidOperationException("Consulta não encontrada entre as agendadas do paciente.");
    }
}
