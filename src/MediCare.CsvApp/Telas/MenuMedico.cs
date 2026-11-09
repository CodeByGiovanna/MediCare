using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

public static class MenuMedico
{
    public static void Abrir(Sistema s, Medico medico)
    {
        Menu.Executar("Médico",
            ("Minha agenda", () => s.Exibir.Consultas(s.Atendimento.Agenda(medico.Id))),
            ("Registrar consulta realizada", () => Concluir(s, medico)),
            ("Histórico de um paciente", () => Historico(s, medico)),
            ("Meus horários", () => s.Exibir.Horarios(s.Disponibilidade.HorariosDoMedico(medico.Id))),
            ("Cadastrar horário", () => CadastrarHorario(s, medico)),
            ("Remover horário", () => RemoverHorario(s, medico)),
            ("Acessar teleconsulta", () => TelaTeleconsulta.Acessar(s, medico)),
            ("Atualizar telefone", () => AtualizarPerfil(s, medico)));
    }

    private static void Concluir(Sistema s, Medico medico)
    {
        s.Exibir.Consultas(s.Atendimento.Agenda(medico.Id));
        s.Atendimento.ConcluirConsulta(medico.Id, Entrada.LerInt("Consulta"));
        Console.WriteLine("Consulta registrada como realizada.");
    }

    private static void Historico(Sistema s, Medico medico)
    {
        var paciente = s.Contas.ObterPorEmail(Entrada.LerTexto("E-mail do paciente")) as Paciente
            ?? throw new InvalidOperationException("Paciente não encontrado.");
        s.Exibir.Consultas(s.Atendimento.HistoricoDoPaciente(medico.Id, paciente.Id));
    }

    private static void CadastrarHorario(Sistema s, Medico medico)
    {
        var data = Entrada.LerData("Data");
        var inicio = Entrada.LerHora("Início");
        var fim = Entrada.LerHora("Término");
        var modalidade = Entrada.Escolher<Modalidade>("Modalidade");

        int? localId = null;
        if (modalidade == Modalidade.Presencial)
        {
            foreach (var l in s.Disponibilidade.LocaisDoMedico(medico.Id))
                Console.WriteLine($"  [{l.Id}] {l.Nome}, {l.Endereco}, {l.Cidade}");
            localId = Entrada.LerInt("Local");
        }

        var horario = s.Disponibilidade.CadastrarHorario(medico.Id, data, inicio, fim, modalidade, localId);
        Console.WriteLine($"Horário {horario.Id} cadastrado.");
    }

    private static void AtualizarPerfil(Sistema s, Medico medico)
    {
        Console.WriteLine($"Telefone atual: {medico.Telefone}");
        s.Contas.AtualizarPerfil(medico, Entrada.LerTexto("Novo telefone"));
        Console.WriteLine("Dados atualizados.");
    }

    private static void RemoverHorario(Sistema s, Medico medico)
    {
        s.Exibir.Horarios(s.Disponibilidade.HorariosDoMedico(medico.Id));
        s.Disponibilidade.RemoverHorario(medico.Id, Entrada.LerInt("Horário"));
        Console.WriteLine("Horário removido.");
    }
}
