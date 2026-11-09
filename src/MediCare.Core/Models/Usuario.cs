using System.Security.Cryptography;
using System.Text;

namespace MediCare.Core.Models;

public abstract class Usuario : IEntidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; private set; } = "";
    public ProvedorAutenticacao Provedor { get; set; } = ProvedorAutenticacao.Local;
    public bool Ativo { get; private set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public void DefinirSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("A senha não pode ser vazia.");
        SenhaHash = GerarHash(senha);
    }

    public bool VerificarSenha(string senha) =>
        Provedor == ProvedorAutenticacao.Local && SenhaHash == GerarHash(senha);

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;

    // Usado apenas ao reconstruir o objeto a partir do CSV.
    internal void Restaurar(string senhaHash, bool ativo)
    {
        SenhaHash = senhaHash;
        Ativo = ativo;
    }

    private static string GerarHash(string senha) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(senha)));
}
