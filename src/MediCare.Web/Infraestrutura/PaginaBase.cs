using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Base das páginas: guarda as mensagens de sucesso e de erro exibidas ao usuário.
/// </summary>
public abstract class PaginaBase : PageModel
{
    [TempData] public string? Mensagem { get; set; }
    [TempData] public string? Erro { get; set; }

    /// <summary>
    /// Executa uma operação e, se uma regra de negócio for violada, mostra o motivo na tela.
    /// </summary>
    protected bool Tentar(Action operacao, string? sucesso = null)
    {
        try
        {
            operacao();
            Mensagem = sucesso;
            return true;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Erro = ex.Message;
            return false;
        }
    }
}
