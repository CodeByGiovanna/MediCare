using MediCare.Core.Models;

namespace MediCare.Tests;

/// <summary>
/// Confirma que o estado das entidades é gravado e lido de volta corretamente,
/// tanto nos arquivos CSV quanto no banco SQLite via EF Core.
/// </summary>
public class PersistenciaTests
{
    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Operacoes_de_agendamento_sao_gravadas(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var reagendada = a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);
        a.Agendamento.Reagendar(reagendada.Id, a.TeleconsultaA.Id);
        var cancelada = a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialB.Id);
        a.Agendamento.Cancelar(cancelada.Id);
        var enviada = a.Notificacoes.ListarPendentes().First();
        a.Notificacoes.MarcarComoEnviada(enviada.Id);

        a.Reabrir();

        var consulta = a.Dados.Consultas.ObterPorId(reagendada.Id)!;
        Assert.Equal(a.TeleconsultaA.Id, consulta.HorarioId);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
        Assert.Equal(StatusConsulta.Cancelada, a.Dados.Consultas.ObterPorId(cancelada.Id)!.Status);

        var horario = a.Dados.Horarios.ObterPorId(a.TeleconsultaA.Id)!;
        Assert.False(horario.Disponivel);
        Assert.Equal(Modalidade.Teleconsulta, horario.Modalidade);
        Assert.Equal(Ambiente.Dia, horario.Data);
        Assert.Equal(new TimeOnly(10, 0), horario.HoraInicio);
        Assert.Null(horario.LocalId);
        Assert.True(a.Dados.Horarios.ObterPorId(a.PresencialA.Id)!.Disponivel);

        var notificacao = a.Dados.Notificacoes.ObterPorId(enviada.Id)!;
        Assert.Equal(StatusNotificacao.Enviada, notificacao.Status);
        Assert.NotNull(notificacao.DataEnvio);
        Assert.Equal(4, a.Dados.Notificacoes.ListarTodos().Count);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Usuarios_sao_gravados_com_senha_e_situacao(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Contas.AlterarSituacao(a.MedicoB.Email, ativo: false);

        a.Reabrir();

        Assert.Equal(4, a.Dados.TodosOsUsuarios().Count());
        Assert.False(a.Dados.Medicos.ObterPorId(a.MedicoB.Id)!.Ativo);
        Assert.IsType<Medico>(a.Contas.Autenticar("carla@teste.com", "senha123"));
        Assert.Equal(CanalNotificacao.Sms, a.Dados.Pacientes.ObterPorId(a.PacienteSms.Id)!.CanalPreferido);
    }
}
