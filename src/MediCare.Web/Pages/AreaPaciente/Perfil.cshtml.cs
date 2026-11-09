using System.ComponentModel.DataAnnotations;
using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.AreaPaciente;

/// <summary>
/// UC08 — Gerenciar perfil (paciente).
/// </summary>
public class PerfilModel(ContaService contas, UsuarioLogado usuarioLogado) : PaginaBase
{
    public Paciente Paciente { get; private set; } = null!;

    [BindProperty, Required(ErrorMessage = "Informe o telefone.")]
    public string Telefone { get; set; } = "";

    [BindProperty]
    public CanalNotificacao Canal { get; set; }

    public void OnGet()
    {
        Paciente = usuarioLogado.ObterPaciente(User);
        Telefone = Paciente.Telefone;
        Canal = Paciente.CanalPreferido;
    }

    public IActionResult OnPost()
    {
        Paciente = usuarioLogado.ObterPaciente(User);
        if (!ModelState.IsValid)
            return Page();

        contas.AtualizarPerfil(Paciente, Telefone, Canal);
        Mensagem = "Dados atualizados.";
        return RedirectToPage();
    }
}
