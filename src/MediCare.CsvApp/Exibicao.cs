using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.CsvApp;

/// <summary>
/// Monta os textos mostrados no terminal a partir dos dados carregados.
/// </summary>
public class Exibicao(MediCareDados dados)
{
    public void Medicos(IEnumerable<Medico> medicos)
    {
        foreach (var m in medicos)
            Console.WriteLine($"  [{m.Id}] {m.Nome} — {Especialidade(m.EspecialidadeId)} — {m.Crm}");
    }

    public void Especialidades()
    {
        foreach (var e in dados.Especialidades.ListarTodos())
            Console.WriteLine($"  [{e.Id}] {e.Nome}");
    }

    public void Horarios(IEnumerable<HorarioDisponivel> horarios)
    {
        var lista = horarios.ToList();
        if (lista.Count == 0)
            Console.WriteLine("  Nenhum horário encontrado.");
        foreach (var h in lista)
            Console.WriteLine($"  [{h.Id}] {Horario(h)}{(h.Disponivel ? "" : " (reservado)")}");
    }

    public void Consultas(IEnumerable<Consulta> consultas)
    {
        var lista = consultas.ToList();
        if (lista.Count == 0)
            Console.WriteLine("  Nenhuma consulta encontrada.");
        foreach (var c in lista)
            Console.WriteLine($"  [{c.Id}] {Consulta(c)}");
    }

    public string Consulta(Consulta c)
    {
        var horario = dados.Horarios.ObterPorId(c.HorarioId)!;
        var medico = dados.Medicos.ObterPorId(horario.MedicoId)!;
        var paciente = dados.Pacientes.ObterPorId(c.PacienteId)!;
        return $"{c.Status} — {paciente.Nome} com Dr(a). {medico.Nome} — {Horario(horario)}";
    }

    public string Horario(HorarioDisponivel h)
    {
        var texto = $"{h.Data:dd/MM/yyyy} {h.HoraInicio:HH:mm}-{h.HoraFim:HH:mm} — {h.Modalidade}";
        if (h.LocalId is int localId && dados.Locais.ObterPorId(localId) is { } local)
            texto += $" — {local.Nome}, {local.Endereco}, {local.Cidade}";
        return texto;
    }

    private string Especialidade(int id) => dados.Especialidades.ObterPorId(id)?.Nome ?? "?";
}
