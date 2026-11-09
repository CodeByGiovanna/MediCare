using System.ComponentModel.DataAnnotations;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Conta;

/// <summary>
/// UC02 — Realizar login.
/// </summary>
public class EntrarModel(ContaService contas) : PaginaBase
{
    [BindProperty, Required(ErrorMessage = "Informe o e-mail.")]
    public string Email { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe a senha."), DataType(DataType.Password)]
    public string Senha { get; set; } = "";

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var usuario = contas.Autenticar(Email, Senha);
            await HttpContext.SignInAsync(UsuarioLogado.CriarIdentidade(usuario));
        }
        catch (InvalidOperationException ex)
        {
            Erro = ex.Message;
            return Page();
        }

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }
}
