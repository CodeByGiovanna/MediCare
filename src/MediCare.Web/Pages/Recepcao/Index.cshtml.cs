using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Web.Pages.Recepcao;

/// <summary>
/// Tela inicial da recepcionista: escolha do paciente atendido no balcão.
/// </summary>
public class IndexModel(MediCareDados dados) : PaginaBase
{
    [BindProperty(SupportsGet = true)]
    public string? Busca { get; set; }

    public IReadOnlyList<Paciente> Pacientes { get; private set; } = [];

    public void OnGet()
    {
        Pacientes = dados.Pacientes.ListarTodos()
            .Where(p => p.Ativo)
            .Where(p => string.IsNullOrWhiteSpace(Busca)
                        || p.Nome.Contains(Busca, StringComparison.OrdinalIgnoreCase)
                        || p.Email.Contains(Busca, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Nome)
            .ToList();
    }
}
