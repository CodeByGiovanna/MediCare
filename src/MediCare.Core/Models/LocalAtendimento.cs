namespace MediCare.Core.Models;

public class LocalAtendimento : IEntidade
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public string Nome { get; set; } = "";
    public string Endereco { get; set; } = "";
    public string Cidade { get; set; } = "";
}
