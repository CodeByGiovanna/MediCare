using MediCare.Core.Models;

namespace MediCare.CsvApp.Telas;

/// <summary>
/// UC11 — Realizar teleconsulta. O sistema libera o acesso ao paciente e ao médico da consulta
/// no horário agendado; a chamada de vídeo é feita pela plataforma de teleconsulta.
/// </summary>
public static class TelaTeleconsulta
{
    public static void Acessar(Sistema s, Usuario usuario)
    {
        s.Exibir.Consultas(s.Atendimento.TeleconsultasAgendadas(usuario));
        var consulta = s.Atendimento.AcessarTeleconsulta(usuario, Entrada.LerInt("Consulta"), DateTime.Now);
        Console.WriteLine($"Acesso liberado: {s.Exibir.Consulta(consulta)}");
    }
}
