using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Caso de uso UC09: o médico cadastra e remove seus horários de atendimento.
/// </summary>
public class DisponibilidadeService(MediCareDados dados)
{
    public IEnumerable<LocalAtendimento> LocaisDoMedico(int medicoId) =>
        dados.Locais.ListarTodos().Where(l => l.MedicoId == medicoId);

    public IEnumerable<HorarioDisponivel> HorariosDoMedico(int medicoId) =>
        dados.Horarios.ListarTodos()
            .Where(h => h.MedicoId == medicoId)
            .OrderBy(h => h.Data).ThenBy(h => h.HoraInicio);

    public HorarioDisponivel CadastrarHorario(int medicoId, DateOnly data, TimeOnly inicio, TimeOnly fim,
        Modalidade modalidade, int? localId)
    {
        if (data < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Não é possível cadastrar horários em datas passadas.");
        if (fim <= inicio)
            throw new ArgumentException("A hora de término deve ser posterior à hora de início.");

        if (modalidade == Modalidade.Presencial)
        {
            var local = localId is int id ? dados.Locais.ObterPorId(id) : null;
            if (local is null || local.MedicoId != medicoId)
                throw new ArgumentException("Informe um local de atendimento cadastrado para este médico.");
        }
        else
        {
            localId = null;
        }

        var horario = new HorarioDisponivel
        {
            MedicoId = medicoId,
            Data = data,
            HoraInicio = inicio,
            HoraFim = fim,
            Modalidade = modalidade,
            LocalId = localId
        };

        if (HorariosDoMedico(medicoId).Any(h => h.ConflitaCom(horario)))
            throw new InvalidOperationException("O horário conflita com outro já cadastrado.");

        dados.Horarios.Adicionar(horario);
        dados.Horarios.Salvar();
        return horario;
    }

    public void RemoverHorario(int medicoId, int horarioId)
    {
        var horario = dados.Horarios.ObterPorId(horarioId);
        if (horario is null || horario.MedicoId != medicoId)
            throw new InvalidOperationException("Horário não encontrado.");
        if (dados.Consultas.ListarTodos().Any(c => c.HorarioId == horarioId))
            throw new InvalidOperationException("O horário possui consultas registradas e não pode ser removido.");

        dados.Horarios.Remover(horarioId);
        dados.Horarios.Salvar();
    }
}
