namespace MediCare.Core.Models;

public enum Modalidade
{
    Presencial,
    Teleconsulta
}

public enum StatusConsulta
{
    Agendada,
    Cancelada,
    Realizada
}

public enum ProvedorAutenticacao
{
    Local,
    Google,
    Microsoft,
    Meta
}

public enum CanalNotificacao
{
    Email,
    Sms,
    WhatsApp
}

public enum TipoNotificacao
{
    Confirmacao,
    Reagendamento,
    Cancelamento,
    Lembrete
}

public enum StatusNotificacao
{
    Pendente,
    Enviada
}
