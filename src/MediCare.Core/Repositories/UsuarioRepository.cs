using MediCare.Core.Csv;
using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

/// <summary>
/// Base dos repositórios de usuários. Trata as colunas herdadas de Usuario;
/// as classes filhas acrescentam apenas as colunas próprias de cada tipo.
/// </summary>
public abstract class UsuarioRepository<T>(string caminho) : CsvRepository<T>(caminho)
    where T : Usuario, new()
{
    private static readonly string[] ColunasUsuario =
        ["Id", "Nome", "Email", "SenhaHash", "Provedor", "Ativo", "DataCadastro"];

    protected virtual string[] ColunasProprias => [];

    protected override string[] Cabecalho => [.. ColunasUsuario, .. ColunasProprias];

    protected virtual void LerCamposProprios(T usuario, IReadOnlyList<string> campos) { }

    protected virtual IEnumerable<string> CamposProprios(T usuario) => [];

    protected override T DeCampos(IReadOnlyList<string> c)
    {
        var usuario = new T
        {
            Id = CsvValor.ParaInt(c[0]),
            Nome = c[1],
            Email = c[2],
            Provedor = CsvValor.ParaEnum<ProvedorAutenticacao>(c[4]),
            DataCadastro = CsvValor.ParaDataHora(c[6])
        };
        usuario.Restaurar(c[3], CsvValor.ParaBool(c[5]));
        LerCamposProprios(usuario, c.Skip(ColunasUsuario.Length).ToList());
        return usuario;
    }

    protected override IEnumerable<string> ParaCampos(T u) =>
    [
        CsvValor.De(u.Id), u.Nome, u.Email, u.SenhaHash, CsvValor.De(u.Provedor),
        CsvValor.De(u.Ativo), CsvValor.De(u.DataCadastro), .. CamposProprios(u)
    ];
}
