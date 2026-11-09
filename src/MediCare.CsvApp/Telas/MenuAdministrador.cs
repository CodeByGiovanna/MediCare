using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

public static class MenuAdministrador
{
    public static void Abrir(Sistema s)
    {
        Menu.Executar("Administrador",
            ("Listar usuários", () => ListarUsuarios(s)),
            ("Ativar usuário", () => AlterarSituacao(s, ativo: true)),
            ("Desativar usuário", () => AlterarSituacao(s, ativo: false)),
            ("Listar agendamentos", () => ListarAgendamentos(s)),
            ("Cancelar agendamento", () => CancelarAgendamento(s)),
            ("Gerar lembretes de amanhã", () => GerarLembretes(s)),
            ("Enviar notificações pendentes", () => EnviarNotificacoes(s)));
    }

    private static void ListarUsuarios(Sistema s)
    {
        foreach (var u in s.Contas.ListarUsuarios())
        {
            var situacao = u.Ativo ? "ativo" : "desativado";
            Console.WriteLine($"  {u.GetType().Name,-14} {u.Nome,-25} {u.Email,-30} {situacao}");
        }
    }

    private static void AlterarSituacao(Sistema s, bool ativo)
    {
        s.Contas.AlterarSituacao(Entrada.LerTexto("E-mail do usuário"), ativo);
        Console.WriteLine(ativo ? "Usuário ativado." : "Usuário desativado.");
    }

    private static void ListarAgendamentos(Sistema s)
    {
        var filtro = Entrada.LerIntOpcional("Situação (1 Agendada, 2 Cancelada, 3 Realizada; Enter para todas)");
        StatusConsulta? status = filtro is int n && Enum.IsDefined(typeof(StatusConsulta), n - 1)
            ? (StatusConsulta)(n - 1)
            : null;
        s.Exibir.Consultas(s.Agendamento.ListarAgendamentos(status));
    }

    private static void CancelarAgendamento(Sistema s)
    {
        s.Exibir.Consultas(s.Agendamento.ListarAgendamentos(StatusConsulta.Agendada));
        s.Agendamento.Cancelar(Entrada.LerInt("Consulta"));
        Console.WriteLine("Consulta cancelada e horário liberado.");
    }

    private static void GerarLembretes(Sistema s)
    {
        var amanha = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var quantidade = s.Notificacoes.GerarLembretes(amanha);
        Console.WriteLine($"{quantidade} lembrete(s) gerado(s) para {amanha:dd/MM/yyyy}.");
    }

    // O envio real depende do serviço externo (e-mail, SMS ou WhatsApp).
    // Aqui as mensagens são exibidas e marcadas como enviadas.
    private static void EnviarNotificacoes(Sistema s)
    {
        var pendentes = s.Notificacoes.ListarPendentes().ToList();
        if (pendentes.Count == 0)
            Console.WriteLine("  Nenhuma notificação pendente.");

        foreach (var n in pendentes)
        {
            Console.WriteLine($"  [{n.Canal} → {n.Destino}] {n.Mensagem}");
            s.Notificacoes.MarcarComoEnviada(n.Id);
        }
    }
}
