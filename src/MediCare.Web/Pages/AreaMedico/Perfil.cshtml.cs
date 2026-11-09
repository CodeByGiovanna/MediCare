using System.ComponentModel.DataAnnotations;
using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.AreaMedico;

/// <summary>
/// UC08 — Gerenciar perfil (médico).
/// </summary>
public class PerfilModel(ContaService contas, UsuarioLogado usuarioLogado) : PaginaBase
{
    public Medico Medico { get; private set; } = null!;

    [BindProperty, Required(ErrorMessage = "Informe o telefone.")]
    public string Telefone { get; set; } = "";

    public void OnGet()
    {
        Medico = usuarioLogado.ObterMedico(User);
        Telefone = Medico.Telefone;
    }

    public IActionResult OnPost()
    {
        Medico = usuarioLogado.ObterMedico(User);
        if (!ModelState.IsValid)
            return Page();

        contas.AtualizarPerfil(Medico, Telefone);
        Mensagem = "Dados atualizados.";
        return RedirectToPage();
    }
}
