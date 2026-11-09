using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

/// <summary>
/// A recepcionista executa os mesmos passos do paciente, informando para qual paciente está atendendo.
/// </summary>
public static class MenuRecepcionista
{
    public static void Abrir(Sistema s)
    {
        var fluxo = new FluxoAgendamento(s);

        Menu.Executar("Recepção",
            ("Pesquisar médicos", fluxo.PesquisarMedicos),
            ("Ver horários de um médico", fluxo.VerHorarios),
            ("Agendar consulta para paciente", () => fluxo.Agendar(EscolherPaciente(s))),
            ("Consultas de um paciente", () => fluxo.ListarConsultas(EscolherPaciente(s))),
            ("Cancelar consulta de paciente", () => fluxo.Cancelar(EscolherPaciente(s))),
            ("Reagendar consulta de paciente", () => fluxo.Reagendar(EscolherPaciente(s))));
    }

    private static Paciente EscolherPaciente(Sistema s)
    {
        var paciente = s.Contas.ObterPorEmail(Entrada.LerTexto("E-mail do paciente")) as Paciente
            ?? throw new InvalidOperationException("Paciente não encontrado.");
        Console.WriteLine($"Paciente: {paciente.Nome}");
        return paciente;
    }
}
