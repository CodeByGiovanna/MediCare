using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Admin;

/// <summary>
/// UC12 — Gerenciar usuários.
/// </summary>
public class UsuariosModel(ContaService contas) : PaginaBase
{
    public IReadOnlyList<Usuario> Usuarios { get; private set; } = [];

    // O administrador logado não pode desativar a própria conta.
    public int AdministradorId => UsuarioLogado.Id(User);

    public void OnGet() => Usuarios = contas.ListarUsuarios().ToList();

    public IActionResult OnPost(string email, bool ativo)
    {
        Tentar(() => contas.AlterarSituacao(email, ativo), ativo ? "Usuário ativado." : "Usuário desativado.");
        return RedirectToPage();
    }
}
