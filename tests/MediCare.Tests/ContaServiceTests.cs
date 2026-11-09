using MediCare.Core.Models;

namespace MediCare.Tests;

public class ContaServiceTests
{
    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Email_ja_usado_por_qualquer_perfil_e_recusado(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        Assert.Throws<InvalidOperationException>(() => a.Contas.CadastrarPaciente(
            "Outra", "CARLA@teste.com", "senha123", "1", new DateOnly(2000, 1, 1), CanalNotificacao.Email));
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Login_com_senha_correta_retorna_o_usuario(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        var usuario = a.Contas.Autenticar("ana@teste.com", "senha123");

        Assert.Equal(a.PacienteEmail.Id, Assert.IsType<Paciente>(usuario).Id);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Login_e_recusado_com_senha_errada_ou_conta_desativada(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        Assert.Throws<InvalidOperationException>(() => a.Contas.Autenticar("ana@teste.com", "errada"));

        a.Contas.AlterarSituacao("ana@teste.com", ativo: false);
        var erro = Assert.Throws<InvalidOperationException>(() => a.Contas.Autenticar("ana@teste.com", "senha123"));
        Assert.Contains("desativada", erro.Message);
    }

    [Theory, MemberData(nameof(Ambiente.Persistencias), MemberType = typeof(Ambiente))]
    public void Perfil_atualizado_e_gravado(string persistencia)
    {
        using var a = new Ambiente(persistencia);

        a.Contas.AtualizarPerfil(a.PacienteEmail, "21900000000", CanalNotificacao.WhatsApp);
        a.Reabrir();

        var paciente = a.Dados.Pacientes.ObterPorId(a.PacienteEmail.Id)!;
        Assert.Equal("21900000000", paciente.Telefone);
        Assert.Equal(CanalNotificacao.WhatsApp, paciente.CanalPreferido);
        Assert.True(paciente.VerificarSenha("senha123"));
    }
}
