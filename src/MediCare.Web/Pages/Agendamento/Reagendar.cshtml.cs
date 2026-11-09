using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Agendamento;

/// <summary>
/// UC07 — Reagendar consulta.
/// </summary>
public class ReagendarModel(MediCareDados dados, AgendamentoService agendamento) : PaginaAgendamento(dados)
{
    public Consulta Consulta { get; private set; } = null!;
    public IReadOnlyList<HorarioDisponivel> Horarios { get; private set; } = [];

    public IActionResult OnGet(int consultaId)
    {
        if (!CarregarPaciente())
            return SemPaciente();
        if (!CarregarConsulta(consultaId))
            return NotFound();

        var medicoId = Dados.Horarios.ObterPorId(Consulta.HorarioId)!.MedicoId;
        Horarios = agendamento.HorariosDisponiveis(medicoId).ToList();
        return Page();
    }

    public IActionResult OnPost(int consultaId, int novoHorarioId)
    {
        if (!CarregarPaciente())
            return SemPaciente();
        if (!CarregarConsulta(consultaId))
            return NotFound();

        if (!Tentar(() => agendamento.Reagendar(consultaId, novoHorarioId), "Consulta reagendada."))
            return RedirectToPage(new { consultaId, pacienteId = PacienteId });

        return RedirectToPage("Consultas", new { pacienteId = PacienteId });
    }

    private bool CarregarConsulta(int consultaId)
    {
        var consulta = Dados.Consultas.ObterPorId(consultaId);
        if (consulta is null || consulta.PacienteId != Paciente.Id || consulta.Status != StatusConsulta.Agendada)
            return false;
        Consulta = consulta;
        return true;
    }
}
