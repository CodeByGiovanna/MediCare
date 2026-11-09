using MediCare.Core.Models;
using MediCare.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Base das páginas de agendamento, usadas pelo paciente e pela recepcionista.
/// O paciente sempre age sobre a própria conta; a recepcionista informa o paciente que está atendendo.
/// </summary>
public abstract class PaginaAgendamento(MediCareDados dados) : PaginaBase
{
    protected MediCareDados Dados { get; } = dados;

    [BindProperty(SupportsGet = true)] public int? PacienteId { get; set; }

    public Paciente Paciente { get; private set; } = null!;

    public bool AtendimentoPelaRecepcao => User.IsInRole(Perfis.Recepcionista);

    protected bool CarregarPaciente()
    {
        var id = User.IsInRole(Perfis.Paciente) ? UsuarioLogado.Id(User) : PacienteId;
        var paciente = id is int i ? Dados.Pacientes.ObterPorId(i) : null;
        if (paciente is null)
            return false;

        Paciente = paciente;
        PacienteId = paciente.Id;
        return true;
    }

    protected IActionResult SemPaciente()
    {
        Erro = "Selecione o paciente que está sendo atendido.";
        return RedirectToPage("/Recepcao/Index");
    }
}
