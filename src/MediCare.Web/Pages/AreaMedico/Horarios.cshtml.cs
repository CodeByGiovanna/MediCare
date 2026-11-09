using System.ComponentModel.DataAnnotations;
using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediCare.Web.Pages.AreaMedico;

/// <summary>
/// UC09 — Gerenciar disponibilidade.
/// </summary>
public class HorariosModel(DisponibilidadeService disponibilidade) : PaginaBase
{
    [BindProperty, Required(ErrorMessage = "Informe a data.")]
    public DateOnly? Data { get; set; }

    [BindProperty, Required(ErrorMessage = "Informe o início.")]
    public TimeOnly? Inicio { get; set; }

    [BindProperty, Required(ErrorMessage = "Informe o término.")]
    public TimeOnly? Fim { get; set; }

    [BindProperty]
    public Modalidade Modalidade { get; set; }

    [BindProperty]
    public int? LocalId { get; set; }

    public IEnumerable<SelectListItem> Locais { get; private set; } = [];
    public IReadOnlyList<HorarioDisponivel> Horarios { get; private set; } = [];

    private int MedicoId => UsuarioLogado.Id(User);

    public void OnGet() => Carregar();

    public IActionResult OnPostCadastrar()
    {
        if (!ModelState.IsValid)
        {
            Carregar();
            return Page();
        }

        if (!Tentar(() => disponibilidade.CadastrarHorario(
                MedicoId, Data!.Value, Inicio!.Value, Fim!.Value, Modalidade, LocalId), "Horário cadastrado."))
        {
            Carregar();
            return Page();
        }
        return RedirectToPage();
    }

    public IActionResult OnPostRemover(int horarioId)
    {
        Tentar(() => disponibilidade.RemoverHorario(MedicoId, horarioId), "Horário removido.");
        return RedirectToPage();
    }

    private void Carregar()
    {
        Locais = disponibilidade.LocaisDoMedico(MedicoId).Select(l => new SelectListItem(l.Nome, l.Id.ToString()));
        Horarios = disponibilidade.HorariosDoMedico(MedicoId).ToList();
    }
}
