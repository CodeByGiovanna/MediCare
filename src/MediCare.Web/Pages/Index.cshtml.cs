using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediCare.Web.Pages;

public class IndexModel : PageModel
{
    // Usuário autenticado vai direto para a tela inicial do seu perfil.
    public IActionResult OnGet()
    {
        if (User.IsInRole(Perfis.Paciente)) return RedirectToPage("/Agendamento/Consultas");
        if (User.IsInRole(Perfis.Recepcionista)) return RedirectToPage("/Recepcao/Index");
        if (User.IsInRole(Perfis.Medico)) return RedirectToPage("/AreaMedico/Index");
        if (User.IsInRole(Perfis.Administrador)) return RedirectToPage("/Admin/Usuarios");
        return Page();
    }
}
