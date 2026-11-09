using MediCare.Core.Models;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;

namespace MediCare.Web.Pages.Teleconsulta;

/// <summary>
/// UC11 — Realizar teleconsulta. O acesso só é liberado ao paciente e ao médico da consulta,
/// dentro do horário agendado.
/// </summary>
public class AcessarModel(AtendimentoService atendimento, UsuarioLogado usuarioLogado) : PaginaBase
{
    public Consulta? Consulta { get; private set; }

    public void OnGet(int consultaId)
    {
        var usuario = usuarioLogado.Obter(User);
        Tentar(() => Consulta = atendimento.AcessarTeleconsulta(usuario, consultaId, DateTime.Now));
    }
}
