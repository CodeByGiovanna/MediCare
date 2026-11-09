using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Textos exibidos nas telas a partir dos identificadores guardados nas entidades.
/// </summary>
public class Apresentacao(MediCareDados dados)
{
    public HorarioDisponivel Horario(Consulta consulta) => dados.Horarios.ObterPorId(consulta.HorarioId)!;

    public string Paciente(Consulta consulta) => dados.Pacientes.ObterPorId(consulta.PacienteId)!.Nome;

    public string Medico(int medicoId) => dados.Medicos.ObterPorId(medicoId)!.Nome;

    public string Medico(Consulta consulta) => Medico(Horario(consulta).MedicoId);

    public string Especialidade(int especialidadeId) =>
        dados.Especialidades.ObterPorId(especialidadeId)?.Nome ?? "";

    public string DataHora(HorarioDisponivel h) =>
        $"{h.Data:dd/MM/yyyy} {h.HoraInicio:HH:mm}–{h.HoraFim:HH:mm}";

    public string Local(HorarioDisponivel h) =>
        h.LocalId is int id && dados.Locais.ObterPorId(id) is { } local
            ? $"{local.Nome} — {local.Endereco}, {local.Cidade}"
            : "Teleconsulta";

    /// <summary>Nome de exibição de um valor de enumeração (atributo Display, quando houver).</summary>
    public static string Nome(Enum valor) =>
        valor.GetType().GetField(valor.ToString())?.GetCustomAttribute<DisplayAttribute>()?.Name ?? valor.ToString();
}
