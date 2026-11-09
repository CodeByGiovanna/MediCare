using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Agendamento;

/// <summary>
/// Consultas do paciente e UC06 — Cancelar consulta.
/// </summary>
public class ConsultasModel(MediCareDados dados, AgendamentoService agendamento, Apresentacao texto)
    : PaginaAgendamento(dados)
{
    public IReadOnlyList<Consulta> Consultas { get; private set; } = [];

    public IActionResult OnGet()
    {
        if (!CarregarPaciente())
            return SemPaciente();

        Consultas = agendamento.ConsultasDoPaciente(Paciente.Id)
            .OrderByDescending(c => texto.Horario(c).Data)
            .ThenByDescending(c => texto.Horario(c).HoraInicio)
            .ToList();
        return Page();
    }

    public IActionResult OnPostCancelar(int consultaId)
    {
        if (!CarregarPaciente())
            return SemPaciente();

        var consulta = Dados.Consultas.ObterPorId(consultaId);
        if (consulta is null || consulta.PacienteId != Paciente.Id)
            return NotFound();

        Tentar(() => agendamento.Cancelar(consultaId), "Consulta cancelada e horário liberado.");
        return RedirectToPage(new { pacienteId = PacienteId });
    }
}
