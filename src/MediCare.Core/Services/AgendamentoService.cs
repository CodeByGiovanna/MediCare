using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Casos de uso UC03 a UC07, usados pelo paciente e pela recepcionista:
/// pesquisar médicos, consultar horários, agendar, cancelar e reagendar consultas.
/// </summary>
public class AgendamentoService(MediCareDados dados, NotificacaoService notificacoes)
{
    public IEnumerable<Medico> PesquisarMedicos(int? especialidadeId = null) =>
        dados.Medicos.ListarTodos()
            .Where(m => m.Ativo && (especialidadeId is null || m.EspecialidadeId == especialidadeId));

    public IEnumerable<HorarioDisponivel> HorariosDisponiveis(int medicoId) =>
        dados.Horarios.ListarTodos()
            .Where(h => h.MedicoId == medicoId && h.Disponivel)
            .OrderBy(h => h.Data).ThenBy(h => h.HoraInicio);

    public IEnumerable<Consulta> ConsultasDoPaciente(int pacienteId) =>
        dados.Consultas.ListarTodos().Where(c => c.PacienteId == pacienteId);

    public Consulta Agendar(int pacienteId, int horarioId)
    {
        var paciente = dados.Pacientes.ObterPorId(pacienteId)
            ?? throw new InvalidOperationException("Paciente não encontrado.");
        if (!paciente.Ativo)
            throw new InvalidOperationException("Paciente desativado não pode agendar consultas.");

        var horario = ObterHorario(horarioId);
        horario.Reservar();

        var consulta = dados.Consultas.Adicionar(new Consulta(pacienteId, horarioId));
        Salvar();
        notificacoes.Registrar(consulta, TipoNotificacao.Confirmacao);
        return consulta;
    }

    public void Cancelar(int consultaId)
    {
        var consulta = ObterConsulta(consultaId);
        consulta.Cancelar();
        ObterHorario(consulta.HorarioId).Liberar();
        Salvar();
        notificacoes.Registrar(consulta, TipoNotificacao.Cancelamento);
    }

    public void Reagendar(int consultaId, int novoHorarioId)
    {
        var consulta = ObterConsulta(consultaId);
        var horarioAtual = ObterHorario(consulta.HorarioId);
        var novoHorario = ObterHorario(novoHorarioId);

        if (novoHorario.MedicoId != horarioAtual.MedicoId)
            throw new InvalidOperationException("O novo horário deve ser do mesmo médico.");
        if (!novoHorario.Disponivel)
            throw new InvalidOperationException("O novo horário não está disponível.");

        consulta.Reagendar(novoHorarioId);
        novoHorario.Reservar();
        horarioAtual.Liberar();
        Salvar();
        notificacoes.Registrar(consulta, TipoNotificacao.Reagendamento);
    }

    private Consulta ObterConsulta(int id) =>
        dados.Consultas.ObterPorId(id) ?? throw new InvalidOperationException("Consulta não encontrada.");

    private HorarioDisponivel ObterHorario(int id) =>
        dados.Horarios.ObterPorId(id) ?? throw new InvalidOperationException("Horário não encontrado.");

    private void Salvar()
    {
        dados.Horarios.Salvar();
        dados.Consultas.Salvar();
    }
}
