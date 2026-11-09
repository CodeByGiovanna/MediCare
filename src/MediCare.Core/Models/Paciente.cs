namespace MediCare.Core.Models;

public class Paciente : Usuario
{
    public string Telefone { get; set; } = "";
    public DateOnly DataNascimento { get; set; }
    public CanalNotificacao CanalPreferido { get; set; } = CanalNotificacao.Email;
}
