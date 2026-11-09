using MediCare.Core.Models;

namespace MediCare.Tests;

public class ModeloTests
{
    [Fact]
    public void Senha_e_guardada_como_hash_e_verificada()
    {
        var paciente = new Paciente();
        paciente.DefinirSenha("segredo1");

        Assert.NotEqual("segredo1", paciente.SenhaHash);
        Assert.True(paciente.VerificarSenha("segredo1"));
        Assert.False(paciente.VerificarSenha("outra"));
    }

    [Fact]
    public void Conta_de_provedor_externo_nao_aceita_senha()
    {
        var paciente = new Paciente { Provedor = ProvedorAutenticacao.Google };
        paciente.DefinirSenha("segredo1");

        Assert.False(paciente.VerificarSenha("segredo1"));
    }

    [Fact]
    public void Horario_nao_pode_ser_reservado_duas_vezes()
    {
        var horario = new HorarioDisponivel();
        horario.Reservar();

        Assert.False(horario.Disponivel);
        Assert.Throws<InvalidOperationException>(horario.Reservar);
    }

    [Fact]
    public void Horarios_sobrepostos_conflitam_e_consecutivos_nao()
    {
        var dia = new DateOnly(2026, 12, 1);
        var nove = new HorarioDisponivel { MedicoId = 1, Data = dia, HoraInicio = new(9, 0), HoraFim = new(9, 30) };
        var noveEQuinze = new HorarioDisponivel { MedicoId = 1, Data = dia, HoraInicio = new(9, 15), HoraFim = new(9, 45) };
        var noveEMeia = new HorarioDisponivel { MedicoId = 1, Data = dia, HoraInicio = new(9, 30), HoraFim = new(10, 0) };
        var outroMedico = new HorarioDisponivel { MedicoId = 2, Data = dia, HoraInicio = new(9, 0), HoraFim = new(9, 30) };

        Assert.True(nove.ConflitaCom(noveEQuinze));
        Assert.False(nove.ConflitaCom(noveEMeia));
        Assert.False(nove.ConflitaCom(outroMedico));
    }

    [Fact]
    public void Consulta_cancelada_nao_pode_ser_alterada()
    {
        var consulta = new Consulta(pacienteId: 1, horarioId: 1);
        consulta.Cancelar();

        Assert.Equal(StatusConsulta.Cancelada, consulta.Status);
        Assert.Throws<InvalidOperationException>(consulta.Cancelar);
        Assert.Throws<InvalidOperationException>(() => consulta.Reagendar(2));
        Assert.Throws<InvalidOperationException>(consulta.Concluir);
    }

    [Fact]
    public void Notificacao_so_pode_ser_enviada_uma_vez()
    {
        var notificacao = new Notificacao(1, TipoNotificacao.Lembrete, CanalNotificacao.Email, "a@b.com", "texto");
        notificacao.MarcarComoEnviada();

        Assert.Equal(StatusNotificacao.Enviada, notificacao.Status);
        Assert.NotNull(notificacao.DataEnvio);
        Assert.Throws<InvalidOperationException>(notificacao.MarcarComoEnviada);
    }
}
