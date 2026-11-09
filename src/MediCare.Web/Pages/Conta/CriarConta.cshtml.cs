using System.ComponentModel.DataAnnotations;
using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Conta;

/// <summary>
/// UC01 — Criar conta (paciente).
/// </summary>
public class CriarContaModel(ContaService contas) : PaginaBase
{
    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe o e-mail."), EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe a senha."), MinLength(6, ErrorMessage = "Use ao menos 6 caracteres."),
     DataType(DataType.Password)]
    public string Senha { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe o telefone.")]
    public string Telefone { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe a data de nascimento.")]
    public DateOnly? DataNascimento { get; set; }

    [BindProperty]
    public CanalNotificacao Canal { get; set; } = CanalNotificacao.Email;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Paciente? paciente = null;
        if (!Tentar(() => paciente = contas.CadastrarPaciente(Nome, Email, Senha, Telefone, DataNascimento!.Value, Canal)))
            return Page();

        await HttpContext.SignInAsync(UsuarioLogado.CriarIdentidade(paciente!));
        Mensagem = "Conta criada. Bem-vindo(a) ao MediCare!";
        return RedirectToPage("/Agendamento/Consultas");
    }
}
