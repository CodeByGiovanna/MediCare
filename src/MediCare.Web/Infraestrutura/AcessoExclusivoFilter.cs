using Microsoft.AspNetCore.Mvc.Filters;

namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Processa uma requisição por vez. Assim, duas pessoas não conseguem reservar o mesmo
/// horário ao mesmo tempo, e a opção de persistência em CSV, que não suporta gravações
/// simultâneas, também fica protegida.
/// </summary>
public class AcessoExclusivoFilter : IAsyncPageFilter
{
    private static readonly SemaphoreSlim Trava = new(1, 1);

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        await Trava.WaitAsync();
        try
        {
            await next();
        }
        finally
        {
            Trava.Release();
        }
    }
}
