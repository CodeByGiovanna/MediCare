using System.Globalization;

namespace MediCare.CsvApp;

/// <summary>
/// Leitura de dados digitados no terminal.
/// </summary>
public static class Entrada
{
    private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

    public static string? Ler(string rotulo)
    {
        Console.Write($"{rotulo}: ");
        return Console.ReadLine()?.Trim();
    }

    public static string LerTexto(string rotulo)
    {
        var valor = Ler(rotulo);
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"{rotulo} é obrigatório.");
        return valor;
    }

    public static int LerInt(string rotulo) =>
        int.TryParse(Ler(rotulo), out var valor) ? valor : throw new FormatException($"{rotulo} deve ser um número.");

    public static int? LerIntOpcional(string rotulo)
    {
        var texto = Ler(rotulo);
        if (string.IsNullOrWhiteSpace(texto))
            return null;
        return int.TryParse(texto, out var valor) ? valor : throw new FormatException($"{rotulo} deve ser um número.");
    }

    public static DateOnly LerData(string rotulo) =>
        DateOnly.TryParseExact(Ler($"{rotulo} (dd/mm/aaaa)"), "dd/MM/yyyy", Cultura, DateTimeStyles.None, out var data)
            ? data
            : throw new FormatException($"{rotulo} inválida.");

    public static TimeOnly LerHora(string rotulo) =>
        TimeOnly.TryParseExact(Ler($"{rotulo} (hh:mm)"), "HH:mm", Cultura, DateTimeStyles.None, out var hora)
            ? hora
            : throw new FormatException($"{rotulo} inválida.");

    public static TEnum Escolher<TEnum>(string rotulo) where TEnum : struct, Enum
    {
        var valores = Enum.GetValues<TEnum>();
        for (var i = 0; i < valores.Length; i++)
            Console.WriteLine($"  {i + 1} - {valores[i]}");

        var opcao = LerInt(rotulo);
        if (opcao < 1 || opcao > valores.Length)
            throw new ArgumentException($"{rotulo}: opção inválida.");
        return valores[opcao - 1];
    }
}
