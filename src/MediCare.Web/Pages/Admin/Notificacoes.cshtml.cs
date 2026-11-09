using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Admin;

/// <summary>
/// UC14 — Enviar notificações. A entrega real depende do serviço externo (e-mail, SMS ou WhatsApp);
/// aqui as notificações pendentes são registradas como entregues a esse serviço.
/// </summary>
public class NotificacoesModel(MediCareDados dados, NotificacaoService notificacoes) : PaginaBase
{
    public IReadOnlyList<Notificacao> Notificacoes { get; private set; } = [];
    public int Pendentes { get; private set; }

    public void OnGet()
    {
        Notificacoes = dados.Notificacoes.ListarTodos().OrderByDescending(n => n.DataCriacao).ToList();
        Pendentes = Notificacoes.Count(n => n.Status == StatusNotificacao.Pendente);
    }

    public IActionResult OnPostGerarLembretes()
    {
        var amanha = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var quantidade = notificacoes.GerarLembretes(amanha);
        Mensagem = $"{quantidade} lembrete(s) gerado(s) para {amanha:dd/MM/yyyy}.";
        return RedirectToPage();
    }

    public IActionResult OnPostEnviar()
    {
        var pendentes = notificacoes.ListarPendentes().ToList();
        foreach (var n in pendentes)
            notificacoes.MarcarComoEnviada(n.Id);
        Mensagem = $"{pendentes.Count} notificação(ões) enviada(s).";
        return RedirectToPage();
    }
}
