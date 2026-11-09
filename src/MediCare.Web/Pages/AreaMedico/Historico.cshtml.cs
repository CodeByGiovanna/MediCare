using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Web.Infraestrutura;

namespace MediCare.Web.Pages.AreaMedico;

/// <summary>
/// UC15 — Consultar histórico do paciente.
/// </summary>
public class HistoricoModel(MediCareDados dados, ContaService contas, AtendimentoService atendimento) : PaginaBase
{
    public string? Email { get; private set; }
    public Paciente? Paciente { get; private set; }
    public IReadOnlyList<Consulta> Consultas { get; private set; } = [];

    public void OnGet(int? pacienteId, string? email)
    {
        Email = email;
        var paciente = pacienteId is int id
            ? dados.Pacientes.ObterPorId(id)
            : string.IsNullOrWhiteSpace(email) ? null : contas.ObterPorEmail(email) as Paciente;

        if (paciente is null)
        {
            if (pacienteId is not null || !string.IsNullOrWhiteSpace(email))
                Erro = "Paciente não encontrado.";
            return;
        }

        Email = paciente.Email;
        if (Tentar(() => Consultas = atendimento.HistoricoDoPaciente(UsuarioLogado.Id(User), paciente.Id).ToList()))
            Paciente = paciente;
    }
}
