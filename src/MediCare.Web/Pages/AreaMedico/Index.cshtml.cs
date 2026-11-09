using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.AreaMedico;

/// <summary>
/// UC10 — Gerenciar agenda.
/// </summary>
public class IndexModel(AtendimentoService atendimento) : PaginaBase
{
    public IReadOnlyList<Consulta> Consultas { get; private set; } = [];

    public void OnGet() => Consultas = atendimento.Agenda(UsuarioLogado.Id(User)).ToList();

    public IActionResult OnPostConcluir(int consultaId)
    {
        Tentar(() => atendimento.ConcluirConsulta(UsuarioLogado.Id(User), consultaId),
            "Consulta registrada como realizada.");
        return RedirectToPage();
    }
}
