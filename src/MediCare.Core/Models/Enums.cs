using System.ComponentModel.DataAnnotations;

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
    [Display(Name = "Senha")] Local,
    Google,
    Microsoft,
    Meta
}

public enum CanalNotificacao
{
    [Display(Name = "E-mail")] Email,
    [Display(Name = "SMS")] Sms,
    WhatsApp
}

public enum TipoNotificacao
{
    [Display(Name = "Confirmação")] Confirmacao,
    Reagendamento,
    Cancelamento,
    Lembrete
}

public enum StatusNotificacao
{
    Pendente,
    Enviada
}
