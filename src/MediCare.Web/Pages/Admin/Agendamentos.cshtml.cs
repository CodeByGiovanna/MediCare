using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Admin;

/// <summary>
/// UC13 — Gerenciar agendamentos.
/// </summary>
public class AgendamentosModel(AgendamentoService agendamento) : PaginaBase
{
    [BindProperty(SupportsGet = true)]
    public StatusConsulta? Status { get; set; }

    public IReadOnlyList<Consulta> Consultas { get; private set; } = [];

    public void OnGet() => Consultas = agendamento.ListarAgendamentos(Status).ToList();

    public IActionResult OnPost(int consultaId)
    {
        Tentar(() => agendamento.Cancelar(consultaId), "Consulta cancelada e horário liberado.");
        return RedirectToPage(new { Status });
    }
}
