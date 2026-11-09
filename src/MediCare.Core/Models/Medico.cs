namespace MediCare.Core.Models;

public class Medico : Usuario
{
    public string Crm { get; set; } = "";
    public string Telefone { get; set; } = "";
    public int EspecialidadeId { get; set; }
}
