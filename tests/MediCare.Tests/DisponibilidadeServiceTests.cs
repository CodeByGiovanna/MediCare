using MediCare.Core.Models;

namespace MediCare.Tests;

public class DisponibilidadeServiceTests
{
    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Recusa_data_passada_e_intervalo_invertido(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        var ontem = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        Assert.Throws<ArgumentException>(() => a.Disponibilidade.CadastrarHorario(
            a.MedicoA.Id, ontem, new(9, 0), new(9, 30), Modalidade.Teleconsulta, null));
        Assert.Throws<ArgumentException>(() => a.Disponibilidade.CadastrarHorario(
            a.MedicoA.Id, Ambiente.Dia, new(15, 0), new(14, 0), Modalidade.Teleconsulta, null));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Presencial_exige_local_do_proprio_medico(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        Assert.Throws<ArgumentException>(() => a.Disponibilidade.CadastrarHorario(
            a.MedicoA.Id, Ambiente.Dia, new(14, 0), new(14, 30), Modalidade.Presencial, null));
        Assert.Throws<ArgumentException>(() => a.Disponibilidade.CadastrarHorario(
            a.MedicoB.Id, Ambiente.Dia, new(14, 0), new(14, 30), Modalidade.Presencial, a.LocalA.Id));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Teleconsulta_nao_guarda_local(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        var horario = a.Disponibilidade.CadastrarHorario(
            a.MedicoA.Id, Ambiente.Dia, new(14, 0), new(14, 30), Modalidade.Teleconsulta, a.LocalA.Id);

        Assert.Null(horario.LocalId);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Recusa_horario_que_conflita_com_outro_do_mesmo_medico(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        Assert.Throws<InvalidOperationException>(() => a.Disponibilidade.CadastrarHorario(
            a.MedicoA.Id, Ambiente.Dia, new(9, 15), new(9, 45), Modalidade.Teleconsulta, null));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Remove_horario_livre_mas_nao_horario_ja_usado(string persistencia)
    {
        using var a = new Ambiente(persistencia);
        a.Agendamento.Agendar(a.PacienteEmail.Id, a.PresencialA.Id);

        Assert.Throws<InvalidOperationException>(() => a.Disponibilidade.RemoverHorario(a.MedicoA.Id, a.PresencialA.Id));

        a.Disponibilidade.RemoverHorario(a.MedicoA.Id, a.TeleconsultaA.Id);
        Assert.Null(a.Dados.Horarios.ObterPorId(a.TeleconsultaA.Id));
    }
}
