using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Gera as notificações de consulta (UC14) e controla o que já foi entregue ao sistema de notificação.
/// </summary>
public class NotificacaoService(MediCareDados dados)
{
    public Notificacao Registrar(Consulta consulta, TipoNotificacao tipo)
    {
        var paciente = dados.Pacientes.ObterPorId(consulta.PacienteId)!;
        var canal = paciente.CanalPreferido;
        var destino = canal == CanalNotificacao.Email ? paciente.Email : paciente.Telefone;

        var notificacao = new Notificacao(consulta.Id, tipo, canal, destino, MontarMensagem(consulta, tipo));
        dados.Notificacoes.Adicionar(notificacao);
        dados.Notificacoes.Salvar();
        return notificacao;
    }

    public int GerarLembretes(DateOnly dia)
    {
        var jaLembradas = dados.Notificacoes.ListarTodos()
            .Where(n => n.Tipo == TipoNotificacao.Lembrete)
            .Select(n => n.ConsultaId)
            .ToHashSet();

        var consultas = dados.Consultas.ListarTodos()
            .Where(c => c.Status == StatusConsulta.Agendada
                        && !jaLembradas.Contains(c.Id)
                        && dados.Horarios.ObterPorId(c.HorarioId)!.Data == dia)
            .ToList();

        foreach (var consulta in consultas)
            Registrar(consulta, TipoNotificacao.Lembrete);

        return consultas.Count;
    }

    public IEnumerable<Notificacao> ListarPendentes() =>
        dados.Notificacoes.ListarTodos().Where(n => n.Status == StatusNotificacao.Pendente);

    public void MarcarComoEnviada(int notificacaoId)
    {
        var notificacao = dados.Notificacoes.ObterPorId(notificacaoId)
            ?? throw new InvalidOperationException("Notificação não encontrada.");
        notificacao.MarcarComoEnviada();
        dados.Notificacoes.Salvar();
    }

    private string MontarMensagem(Consulta consulta, TipoNotificacao tipo)
    {
        var horario = dados.Horarios.ObterPorId(consulta.HorarioId)!;
        var medico = dados.Medicos.ObterPorId(horario.MedicoId)!;
        var quando = $"{horario.Data:dd/MM/yyyy} às {horario.HoraInicio:HH:mm} com Dr(a). {medico.Nome}";

        var onde = horario.LocalId is int localId && dados.Locais.ObterPorId(localId) is { } local
            ? $" Local: {local.Nome}, {local.Endereco}, {local.Cidade}."
            : " Atendimento por teleconsulta.";

        return tipo switch
        {
            TipoNotificacao.Confirmacao => $"Consulta confirmada para {quando}.{onde}",
            TipoNotificacao.Reagendamento => $"Consulta reagendada para {quando}.{onde}",
            TipoNotificacao.Cancelamento => $"A consulta de {quando} foi cancelada.",
            TipoNotificacao.Lembrete => $"Lembrete: consulta em {quando}.{onde}",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
    }
}
