using System.ComponentModel.DataAnnotations;
using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediCare.Web.Pages.Conta;

/// <summary>
/// UC01 — Criar conta (médico).
/// </summary>
public class CriarContaMedicoModel(ContaService contas, MediCareDados dados) : PaginaBase
{
    [BindProperty, Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe o e-mail."), EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe a senha."), MinLength(6, ErrorMessage = "Use ao menos 6 caracteres."),
     DataType(DataType.Password)]
    public string Senha { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe o CRM.")]
    public string Crm { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Informe o telefone.")]
    public string Telefone { get; set; } = "";

    [BindProperty]
    public int EspecialidadeId { get; set; }

    public IEnumerable<SelectListItem> Especialidades =>
        dados.Especialidades.ListarTodos().Select(e => new SelectListItem(e.Nome, e.Id.ToString()));

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Medico? medico = null;
        if (!Tentar(() => medico = contas.CadastrarMedico(Nome, Email, Senha, Crm, Telefone, EspecialidadeId)))
            return Page();

        await HttpContext.SignInAsync(UsuarioLogado.CriarIdentidade(medico!));
        Mensagem = "Cadastro concluído. Cadastre seus horários de atendimento.";
        return RedirectToPage("/AreaMedico/Horarios");
    }
}
