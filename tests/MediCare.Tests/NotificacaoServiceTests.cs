using MediCare.Core.Models;

namespace MediCare.Tests;

public class NotificacaoServiceTests
{
    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Lembrete_e_gerado_uma_unica_vez_por_consulta(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);
        a.Agendamento.Agendar(a.PacienteSms.Id, a.PresencialB.Id);

        Assert.Equal(2, a.Notificacoes.GerarLembretes(Ambiente.Dia));
        Assert.Equal(0, a.Notificacoes.GerarLembretes(Ambiente.Dia));
        Assert.Equal(0, a.Notificacoes.GerarLembretes(Ambiente.Dia.AddDays(1)));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Notificacao_enviada_sai_da_lista_de_pendentes(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);
        var pendente = Assert.Single(a.Notificacoes.ListarPendentes());

        a.Notificacoes.MarcarComoEnviada(pendente.Id);

        Assert.Empty(a.Notificacoes.ListarPendentes());
        Assert.Throws<InvalidOperationException>(() => a.Notificacoes.MarcarComoEnviada(pendente.Id));
    }
}
