using System.Globalization;

namespace MediCare.Core.Csv;

/// <summary>
/// Conversões entre texto do CSV e tipos .NET, sempre em formato invariável
/// (datas ISO, ponto como separador decimal), para o arquivo não depender
/// da configuração regional da máquina.
/// </summary>
public static class CsvValor
{
    private const string FormatoData = "yyyy-MM-dd";
    private const string FormatoHora = "HH:mm";
    private const string FormatoDataHora = "yyyy-MM-ddTHH:mm:ss";
    private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

    public static string De(int valor) => valor.ToString(Cultura);
    public static string De(int? valor) => valor?.ToString(Cultura) ?? "";
    public static string De(bool valor) => valor ? "true" : "false";
    public static string De(DateOnly valor) => valor.ToString(FormatoData, Cultura);
    public static string De(TimeOnly valor) => valor.ToString(FormatoHora, Cultura);
    public static string De(DateTime valor) => valor.ToString(FormatoDataHora, Cultura);
    public static string De(DateTime? valor) => valor is null ? "" : De(valor.Value);
    public static string De<TEnum>(TEnum valor) where TEnum : struct, Enum => valor.ToString();

    public static int ParaInt(string texto) => int.Parse(texto, Cultura);
    public static int? ParaIntOpcional(string texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : int.Parse(texto, Cultura);
    public static bool ParaBool(string texto) => bool.Parse(texto);
    public static DateOnly ParaData(string texto) => DateOnly.ParseExact(texto, FormatoData, Cultura);
    public static TimeOnly ParaHora(string texto) => TimeOnly.ParseExact(texto, FormatoHora, Cultura);
    public static DateTime ParaDataHora(string texto) => DateTime.ParseExact(texto, FormatoDataHora, Cultura);
    public static DateTime? ParaDataHoraOpcional(string texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : ParaDataHora(texto);
    public static TEnum ParaEnum<TEnum>(string texto) where TEnum : struct, Enum =>
        Enum.Parse<TEnum>(texto, ignoreCase: true);
}
