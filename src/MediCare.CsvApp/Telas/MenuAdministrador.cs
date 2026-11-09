namespace MediCare.CsvApp.Telas;

public static class MenuAdministrador
{
    public static void Abrir(Sistema s)
    {
        Menu.Executar("Administrador",
            ("Listar usuários", () => ListarUsuarios(s)),
            ("Ativar usuário", () => AlterarSituacao(s, ativo: true)),
            ("Desativar usuário", () => AlterarSituacao(s, ativo: false)),
            ("Listar agendamentos", () => s.Exibir.Consultas(s.Dados.Consultas.ListarTodos())),
            ("Gerar lembretes de amanhã", () => GerarLembretes(s)),
            ("Enviar notificações pendentes", () => EnviarNotificacoes(s)));
    }

    private static void ListarUsuarios(Sistema s)
    {
        foreach (var u in s.Contas.ListarUsuarios())
            Console.WriteLine($"  {u.GetType().Name,-14} {u.Nome,-25} {u.Email,-30} {(u.Ativo ? "ativo" : "desativado")}");
    }

    private static void AlterarSituacao(Sistema s, bool ativo)
    {
        s.Contas.AlterarSituacao(Entrada.LerTexto("E-mail do usuário"), ativo);
        Console.WriteLine(ativo ? "Usuário ativado." : "Usuário desativado.");
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
