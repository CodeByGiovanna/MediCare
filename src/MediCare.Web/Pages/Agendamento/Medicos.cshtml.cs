using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediCare.Web.Pages.Agendamento;

/// <summary>
/// UC03 — Pesquisar médicos.
/// </summary>
public class MedicosModel(MediCareDados dados, AgendamentoService agendamento) : PaginaAgendamento(dados)
{
    public IEnumerable<SelectListItem> Especialidades { get; private set; } = [];
    public IReadOnlyList<Medico> Medicos { get; private set; } = [];

    public IActionResult OnGet(int? especialidadeId)
    {
        if (!CarregarPaciente())
            return SemPaciente();

        Especialidades = Dados.Especialidades.ListarTodos()
            .Select(e => new SelectListItem(e.Nome, e.Id.ToString(), e.Id == especialidadeId));
        Medicos = agendamento.PesquisarMedicos(especialidadeId).ToList();
        return Page();
    }
}
