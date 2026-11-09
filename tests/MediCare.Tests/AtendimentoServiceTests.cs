using MediCare.Core.Models;

namespace MediCare.Tests;

public class AtendimentoServiceTests
{
    private static readonly DateTime InicioDaTeleconsulta = Ambiente.Dia.ToDateTime(new TimeOnly(10, 0));

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Agenda_mostra_apenas_consultas_agendadas_do_medico(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.TeleconsultaA.Id);
        a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialA.Id);
        var cancelada = a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialB.Id);
        a.Agendamento.Cancelar(cancelada.Id);

        var agenda = a.Atendimento.Agenda(a.MedicoA.Id).ToList();

        Assert.Equal(new[] { a.PresencialA.Id, a.TeleconsultaA.Id }, agenda.Select(c => c.HorarioId));
        Assert.Empty(a.Atendimento.Agenda(a.MedicoB.Id));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Medico_so_conclui_consultas_da_propria_agenda(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        Assert.Throws<InvalidOperationException>(() => a.Atendimento.ConcluirConsulta(a.MedicoB.Id, consulta.Id));

        a.Atendimento.ConcluirConsulta(a.MedicoA.Id, consulta.Id);
        Assert.Equal(StatusConsulta.Realizada, consulta.Status);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Historico_traz_consultas_realizadas_somente_para_medico_do_paciente(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var realizada = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);
        a.Atendimento.ConcluirConsulta(a.MedicoA.Id, realizada.Id);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.TeleconsultaA.Id);

        var historico = a.Atendimento.HistoricoDoPaciente(a.MedicoA.Id, a.PacienteEmail.Id);

        Assert.Equal(realizada.Id, Assert.Single(historico).Id);
        Assert.Throws<InvalidOperationException>(() => a.Atendimento.HistoricoDoPaciente(a.MedicoB.Id, a.PacienteEmail.Id));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Teleconsulta_e_liberada_aos_participantes_no_horario(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.TeleconsultaA.Id);
        var duranteAConsulta = InicioDaTeleconsulta.AddMinutes(15);

        Assert.Equal(consulta.Id, a.Atendimento.AcessarTeleconsulta(a.PacienteEmail, consulta.Id, duranteAConsulta).Id);
        Assert.Equal(consulta.Id, a.Atendimento.AcessarTeleconsulta(a.MedicoA, consulta.Id, duranteAConsulta).Id);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Teleconsulta_e_recusada_fora_do_horario_ou_para_quem_nao_participa(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.TeleconsultaA.Id);
        var duranteAConsulta = InicioDaTeleconsulta.AddMinutes(15);

        Assert.Throws<InvalidOperationException>(() =>
            a.Atendimento.AcessarTeleconsulta(a.PacienteEmail, consulta.Id, InicioDaTeleconsulta.AddMinutes(-10)));
        Assert.Throws<InvalidOperationException>(() =>
            a.Atendimento.AcessarTeleconsulta(a.PacienteSms, consulta.Id, duranteAConsulta));
        Assert.Throws<InvalidOperationException>(() =>
            a.Atendimento.AcessarTeleconsulta(a.MedicoB, consulta.Id, duranteAConsulta));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Consulta_presencial_nao_tem_acesso_remoto(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        var erro = Assert.Throws<InvalidOperationException>(() => a.Atendimento.AcessarTeleconsulta(
            a.PacienteEmail, consulta.Id, Ambiente.Dia.ToDateTime(new TimeOnly(9, 10))));
        Assert.Contains("presencial", erro.Message);
    }
}
