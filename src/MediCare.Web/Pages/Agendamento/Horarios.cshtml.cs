using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Agendamento;

/// <summary>
/// UC04 — Consultar horários disponíveis e UC05 — Agendar consulta.
/// </summary>
public class HorariosModel(MediCareDados dados, AgendamentoService agendamento) : PaginaAgendamento(dados)
{
    public Medico Medico { get; private set; } = null!;
    public IReadOnlyList<HorarioDisponivel> Horarios { get; private set; } = [];

    public IActionResult OnGet(int medicoId)
    {
        if (!CarregarPaciente())
            return SemPaciente();
        if (Dados.Medicos.ObterPorId(medicoId) is not { } medico)
            return NotFound();

        Medico = medico;
        Horarios = agendamento.HorariosDisponiveis(medicoId).ToList();
        return Page();
    }

    public IActionResult OnPost(int medicoId, int horarioId)
    {
        if (!CarregarPaciente())
            return SemPaciente();

        var sucesso = $"Consulta agendada. A confirmação foi enviada por {Apresentacao.Nome(Paciente.CanalPreferido)}.";
        if (!Tentar(() => agendamento.Agendar(Paciente.Id, horarioId), sucesso))
            return RedirectToPage(new { medicoId, pacienteId = PacienteId });

        return RedirectToPage("Consultas", new { pacienteId = PacienteId });
    }
}
