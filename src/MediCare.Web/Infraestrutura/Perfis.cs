namespace MediCare.Web.Infraestrutura;

/// <summary>
/// Nomes dos perfis de acesso (roles) e das políticas de autorização.
/// </summary>
public static class Perfis
{
    public const string Paciente = "Paciente";
    public const string Recepcionista = "Recepcionista";
    public const string Medico = "Medico";
    public const string Administrador = "Administrador";

    // Políticas que reúnem mais de um perfil.
    public const string Agendamento = "Agendamento";
    public const string Teleconsulta = "Teleconsulta";
}
