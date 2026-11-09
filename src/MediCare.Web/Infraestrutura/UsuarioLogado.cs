using System.Security.Claims;
using MediCare.Core.Models;
using MediCare.Core.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Converte o usuário autenticado (cookie) no objeto do modelo e vice-versa.
/// </summary>
public class UsuarioLogado(MediCareDados dados)
{
    public static ClaimsPrincipal CriarIdentidade(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, Perfil(usuario))
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    public static string Perfil(Usuario usuario) => usuario switch
    {
        Paciente => Perfis.Paciente,
        Recepcionista => Perfis.Recepcionista,
        Medico => Perfis.Medico,
        Administrador => Perfis.Administrador,
        _ => throw new ArgumentOutOfRangeException(nameof(usuario))
    };

    public static int Id(ClaimsPrincipal principal) =>
        int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public Usuario Obter(ClaimsPrincipal principal)
    {
        var id = Id(principal);
        Usuario? usuario =
            principal.IsInRole(Perfis.Paciente) ? dados.Pacientes.ObterPorId(id) :
            principal.IsInRole(Perfis.Recepcionista) ? dados.Recepcionistas.ObterPorId(id) :
            principal.IsInRole(Perfis.Medico) ? dados.Medicos.ObterPorId(id) :
            principal.IsInRole(Perfis.Administrador) ? dados.Administradores.ObterPorId(id) :
            null;
        return usuario ?? throw new InvalidOperationException("Usuário não encontrado.");
    }

    public Paciente ObterPaciente(ClaimsPrincipal principal) => (Paciente)Obter(principal);

    public Medico ObterMedico(ClaimsPrincipal principal) => (Medico)Obter(principal);
}
