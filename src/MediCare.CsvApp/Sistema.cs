using MediCare.Core.Repositories;
using MediCare.Core.Services;

namespace MediCare.CsvApp;

/// <summary>
/// Carrega os dados e cria os serviços usados pelos menus.
/// </summary>
public class Sistema
{
    public MediCareDados Dados { get; }
    public ContaService Contas { get; }
    public NotificacaoService Notificacoes { get; }
    public AgendamentoService Agendamento { get; }
    public DisponibilidadeService Disponibilidade { get; }
    public AtendimentoService Atendimento { get; }
    public Exibicao Exibir { get; }

    public Sistema(string pasta)
    {
        Dados = new MediCareDados(pasta);
        Contas = new ContaService(Dados);
        Notificacoes = new NotificacaoService(Dados);
        Agendamento = new AgendamentoService(Dados, Notificacoes);
        Disponibilidade = new DisponibilidadeService(Dados);
        Atendimento = new AtendimentoService(Dados);
        Exibir = new Exibicao(Dados);
    }
}
