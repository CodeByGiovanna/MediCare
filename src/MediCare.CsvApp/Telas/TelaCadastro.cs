using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

/// <summary>
/// UC01 — Criar conta.
/// </summary>
public static class TelaCadastro
{
    public static void Paciente(Sistema s)
    {
        var paciente = s.Contas.CadastrarPaciente(
            Entrada.LerTexto("Nome"),
            Entrada.LerTexto("E-mail"),
            Entrada.LerTexto("Senha"),
            Entrada.LerTexto("Telefone"),
            Entrada.LerData("Data de nascimento"),
            Entrada.Escolher<CanalNotificacao>("Canal para receber avisos"));

        Console.WriteLine($"Conta criada. Seu código de paciente é {paciente.Id}.");
    }

    public static void Medico(Sistema s)
    {
        var nome = Entrada.LerTexto("Nome");
        var email = Entrada.LerTexto("E-mail");
        var senha = Entrada.LerTexto("Senha");
        var crm = Entrada.LerTexto("CRM");
        var telefone = Entrada.LerTexto("Telefone");
        s.Exibir.Especialidades();
        var especialidadeId = Entrada.LerInt("Especialidade");

        var medico = s.Contas.CadastrarMedico(nome, email, senha, crm, telefone, especialidadeId);
        Console.WriteLine($"Conta criada. Seu código de médico é {medico.Id}.");
    }
}
