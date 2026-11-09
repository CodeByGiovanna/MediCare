using System.Text;

namespace MediCare.Core.Csv;

/// <summary>
/// Leitura e escrita de CSV no formato RFC 4180: campos separados por vírgula,
/// e campos com vírgula, aspas ou quebra de linha ficam entre aspas duplas.
/// </summary>
public static class CsvParser
{
    public const char Separador = ',';

    public static List<List<string>> Ler(TextReader leitor)
    {
        var registros = new List<List<string>>();
        var registro = new List<string>();
        var campo = new StringBuilder();
        var entreAspas = false;
        int c;

        while ((c = leitor.Read()) != -1)
        {
            var ch = (char)c;

            if (entreAspas)
            {
                if (ch == '"')
                {
                    if (leitor.Peek() == '"')
                    {
                        campo.Append('"');
                        leitor.Read();
                    }
                    else
                    {
                        entreAspas = false;
                    }
                }
                else
                {
                    campo.Append(ch);
                }
                continue;
            }

            switch (ch)
            {
                case '"':
                    entreAspas = true;
                    break;
                case Separador:
                    registro.Add(campo.ToString());
                    campo.Clear();
                    break;
                case '\r':
                    break;
                case '\n':
                    registro.Add(campo.ToString());
                    campo.Clear();
                    AdicionarSeNaoVazio(registros, registro);
                    registro = new List<string>();
                    break;
                default:
                    campo.Append(ch);
                    break;
            }
        }

        if (entreAspas)
            throw new FormatException("Arquivo CSV inválido: aspas não fechadas.");

        if (campo.Length > 0 || registro.Count > 0)
        {
            registro.Add(campo.ToString());
            AdicionarSeNaoVazio(registros, registro);
        }

        return registros;
    }

    public static string FormatarLinha(IEnumerable<string> campos) =>
        string.Join(Separador, campos.Select(Escapar));

    private static string Escapar(string valor)
    {
        if (valor.IndexOfAny(new[] { Separador, '"', '\n', '\r' }) < 0)
            return valor;
        return $"\"{valor.Replace("\"", "\"\"")}\"";
    }

    private static void AdicionarSeNaoVazio(List<List<string>> registros, List<string> registro)
    {
        var linhaEmBranco = registro.Count == 1 && registro[0].Length == 0;
        if (!linhaEmBranco)
            registros.Add(registro);
    }
}
