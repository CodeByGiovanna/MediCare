using System.Text;
using MediCare.Core.Csv;
using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

/// <summary>
/// Repositório genérico que mantém uma entidade em um arquivo CSV.
/// Os registros são carregados na criação do repositório e gravados em Salvar().
/// Cada classe filha define o cabeçalho e como converter uma linha em objeto e vice-versa.
/// </summary>
public abstract class CsvRepository<T> where T : class, IEntidade
{
    private readonly string _caminho;
    private readonly List<T> _itens = new();

    protected CsvRepository(string caminho)
    {
        _caminho = caminho;
        Carregar();
    }

    protected abstract string[] Cabecalho { get; }
    protected abstract T DeCampos(IReadOnlyList<string> campos);
    protected abstract IEnumerable<string> ParaCampos(T item);

    public IReadOnlyList<T> ListarTodos() => _itens;

    public T? ObterPorId(int id) => _itens.FirstOrDefault(i => i.Id == id);

    public T Adicionar(T item)
    {
        item.Id = _itens.Count == 0 ? 1 : _itens.Max(i => i.Id) + 1;
        _itens.Add(item);
        return item;
    }

    public bool Remover(int id) => _itens.RemoveAll(i => i.Id == id) > 0;

    public void Salvar()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_caminho))!);

        var conteudo = new StringBuilder();
        conteudo.AppendLine(CsvParser.FormatarLinha(Cabecalho));
        foreach (var item in _itens.OrderBy(i => i.Id))
            conteudo.AppendLine(CsvParser.FormatarLinha(ParaCampos(item)));

        // Grava primeiro em um arquivo temporário para não corromper o original em caso de falha.
        var temporario = _caminho + ".tmp";
        File.WriteAllText(temporario, conteudo.ToString(), new UTF8Encoding(false));
        File.Move(temporario, _caminho, overwrite: true);
    }

    private void Carregar()
    {
        if (!File.Exists(_caminho))
            return;

        using var leitor = new StreamReader(_caminho, Encoding.UTF8);
        var registros = CsvParser.Ler(leitor);
        if (registros.Count == 0)
            return;

        ValidarCabecalho(registros[0]);

        for (var i = 1; i < registros.Count; i++)
        {
            var campos = registros[i];
            if (campos.Count != Cabecalho.Length)
                throw Erro(i, $"esperados {Cabecalho.Length} campos, encontrados {campos.Count}");

            try
            {
                _itens.Add(DeCampos(campos));
            }
            catch (FormatException ex)
            {
                throw Erro(i, ex.Message);
            }
        }
    }

    private void ValidarCabecalho(IReadOnlyList<string> cabecalhoArquivo)
    {
        if (!cabecalhoArquivo.SequenceEqual(Cabecalho, StringComparer.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Cabeçalho inválido em {Path.GetFileName(_caminho)}. Esperado: {string.Join(",", Cabecalho)}");
    }

    private InvalidDataException Erro(int registro, string detalhe) =>
        new($"{Path.GetFileName(_caminho)}, registro {registro}: {detalhe}");
}
