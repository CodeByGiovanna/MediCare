using MediCare.Core.Models;

namespace MediCare.Tests;

public class AgendamentoServiceTests
{
    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Agendar_reserva_o_horario_e_gera_confirmacao_no_canal_do_paciente(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        var consulta = a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialA.Id);

        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
        Assert.DoesNotContain(a.Agendamento.HorariosDisponiveis(a.MedicoA.Id), h => h.Id == a.PresencialA.Id);

        var notificacao = Assert.Single(a.Notificacoes.ListarPendentes());
        Assert.Equal(TipoNotificacao.Confirmacao, notificacao.Tipo);
        Assert.Equal(CanalNotificacao.Sms, notificacao.Canal);
        Assert.Equal("21922222222", notificacao.Destino);
        Assert.Contains("Clínica A", notificacao.Mensagem);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Horario_ja_reservado_nao_pode_ser_agendado(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        Assert.Throws<InvalidOperationException>(() => a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialA.Id));
        Assert.Single(a.Dados.Consultas.ListarTodos());
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Paciente_desativado_nao_agenda(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Contas.AlterarSituacao(a.PacienteEmail.Email, ativo: false);

        Assert.Throws<InvalidOperationException>(() => a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Cancelar_libera_o_horario_e_avisa_o_paciente(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        a.Agendamento.Cancelar(consulta.Id);

        Assert.Equal(StatusConsulta.Cancelada, consulta.Status);
        Assert.Contains(a.Agendamento.HorariosDisponiveis(a.MedicoA.Id), h => h.Id == a.PresencialA.Id);
        Assert.Contains(a.Notificacoes.ListarPendentes(), n => n.Tipo == TipoNotificacao.Cancelamento);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Reagendar_troca_o_horario_e_libera_o_anterior(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        a.Agendamento.Reagendar(consulta.Id, a.TeleconsultaA.Id);

        Assert.Equal(a.TeleconsultaA.Id, consulta.HorarioId);
        Assert.True(a.Dados.Horarios.ObterPorId(a.PresencialA.Id)!.Disponivel);
        Assert.False(a.Dados.Horarios.ObterPorId(a.TeleconsultaA.Id)!.Disponivel);
        Assert.Contains(a.Notificacoes.ListarPendentes(), n => n.Tipo == TipoNotificacao.Reagendamento);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Reagendar_para_outro_medico_e_recusado_sem_alterar_nada(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var consulta = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        Assert.Throws<InvalidOperationException>(() => a.Agendamento.Reagendar(consulta.Id, a.PresencialB.Id));
        Assert.Equal(a.PresencialA.Id, consulta.HorarioId);
        Assert.True(a.Dados.Horarios.ObterPorId(a.PresencialB.Id)!.Disponivel);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Listagem_de_agendamentos_filtra_por_situacao(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var primeira = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);
        a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialB.Id);
        a.Agendamento.Cancelar(primeira.Id);

        Assert.Equal(2, a.Agendamento.ListarAgendamentos().Count());
        Assert.Single(a.Agendamento.ListarAgendamentos(StatusConsulta.Cancelada));
        Assert.Single(a.Agendamento.ListarAgendamentos(StatusConsulta.Agendada));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Pesquisa_ignora_medicos_desativados(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Contas.AlterarSituacao(a.MedicoB.Email, ativo: false);

        var medico = Assert.Single(a.Agendamento.PesquisarMedicos());
        Assert.Equal(a.MedicoA.Id, medico.Id);
    }
}
