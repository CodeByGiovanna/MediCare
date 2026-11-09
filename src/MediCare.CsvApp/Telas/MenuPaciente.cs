using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

public static class MenuPaciente
{
    public static void Abrir(Sistema s, Paciente paciente)
    {
        var fluxo = new FluxoAgendamento(s);

        Menu.Executar("Paciente",
            ("Pesquisar médicos", fluxo.PesquisarMedicos),
            ("Ver horários de um médico", fluxo.VerHorarios),
            ("Agendar consulta", () => fluxo.Agendar(paciente)),
            ("Minhas consultas", () => fluxo.ListarConsultas(paciente)),
            ("Cancelar consulta", () => fluxo.Cancelar(paciente)),
            ("Reagendar consulta", () => fluxo.Reagendar(paciente)),
            ("Atualizar contato", () => AtualizarContato(s, paciente)));
    }

    private static void AtualizarContato(Sistema s, Paciente paciente)
    {
        Console.WriteLine($"Telefone atual: {paciente.Telefone} — canal atual: {paciente.CanalPreferido}");
        s.Contas.AtualizarContato(paciente,
            Entrada.LerTexto("Novo telefone"),
            Entrada.Escolher<CanalNotificacao>("Canal para receber avisos"));
        Console.WriteLine("Dados atualizados.");
    }
}
