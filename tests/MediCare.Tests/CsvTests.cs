using MediCare.Core.Csv;
using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Tests;

public class CsvTests : IDisposable
{
    private readonly string _pasta = Path.Combine(Path.GetTempPath(), "medicare-csv-" + Guid.NewGuid());

    public CsvTests() => Directory.CreateDirectory(_pasta);

    public void Dispose() => Directory.Delete(_pasta, recursive: true);

    [Fact]
    public void Campos_com_virgula_aspas_e_quebra_de_linha_voltam_iguais()
    {
        string[] campos = ["1", "Rua A, 10", "Espaço \"Crescer\"", "linha 1\nlinha 2", ""];

        var linha = CsvParser.FormatarLinha(campos);
        var lidos = CsvParser.Ler(new StringReader(linha));

        Assert.Equal(campos, Assert.Single(lidos));
    }

    [Fact]
    public void Valor_simples_nao_recebe_aspas()
    {
        Assert.Equal("1,Cardiologia,true", CsvParser.FormatarLinha(["1", "Cardiologia", "true"]));
    }

    [Fact]
    public void Aspas_nao_fechadas_geram_erro()
    {
        Assert.Throws<FormatException>(() => CsvParser.Ler(new StringReader("1,\"sem fim")));
    }

    [Fact]
    public void Repositorio_gera_ids_em_sequencia_e_regrava_o_arquivo()
    {
        var caminho = Path.Combine(_pasta, "especialidades.csv");
        var repositorio = new EspecialidadeRepository(caminho);

        var primeira = repositorio.Adicionar(new Especialidade { Nome = "Cardiologia" });
        var segunda = repositorio.Adicionar(new Especialidade { Nome = "Clínica Geral, adulto" });
        repositorio.Salvar();

        Assert.Equal(1, primeira.Id);
        Assert.Equal(2, segunda.Id);

        var relido = new EspecialidadeRepository(caminho);
        Assert.Equal(new[] { "Cardiologia", "Clínica Geral, adulto" }, relido.ListarTodos().Select(e => e.Nome));
    }

    [Fact]
    public void Cabecalho_diferente_do_esperado_e_recusado()
    {
        var caminho = Path.Combine(_pasta, "especialidades.csv");
        File.WriteAllText(caminho, "Codigo,Descricao\n1,Cardiologia\n");

        Assert.Throws<InvalidDataException>(() => new EspecialidadeRepository(caminho));
    }

    [Fact]
    public void Registro_com_data_invalida_informa_o_arquivo()
    {
        var caminho = Path.Combine(_pasta, "horarios.csv");
        File.WriteAllText(caminho,
            "Id,MedicoId,Data,HoraInicio,HoraFim,Modalidade,LocalId,Disponivel\n" +
            "1,1,2026-13-01,09:00,09:30,Presencial,1,true\n");

        var erro = Assert.Throws<InvalidDataException>(() => new HorarioRepository(caminho));
        Assert.Contains("horarios.csv", erro.Message);
    }
}
