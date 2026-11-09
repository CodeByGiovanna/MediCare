namespace MediCare.Core.Models;

/// <summary>
/// Mensagem gerada pelo sistema para ser entregue ao paciente pelo sistema de notificação
/// (e-mail, SMS ou WhatsApp).
/// </summary>
public class Notificacao : IEntidade
{
    public int Id { get; set; }
    public int ConsultaId { get; private set; }
    public TipoNotificacao Tipo { get; private set; }
    public CanalNotificacao Canal { get; private set; }
    public string Destino { get; private set; }
    public string Mensagem { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public StatusNotificacao Status { get; private set; } = StatusNotificacao.Pendente;
    public DateTime? DataEnvio { get; private set; }

    // Usado pelo EF Core ao materializar a entidade.
    private Notificacao()
    {
        Destino = "";
        Mensagem = "";
    }

    public Notificacao(int consultaId, TipoNotificacao tipo, CanalNotificacao canal, string destino, string mensagem)
    {
        ConsultaId = consultaId;
        Tipo = tipo;
        Canal = canal;
        Destino = destino;
        Mensagem = mensagem;
        DataCriacao = DateTime.Now;
    }

    internal Notificacao(int id, int consultaId, TipoNotificacao tipo, CanalNotificacao canal, string destino,
        string mensagem, DateTime dataCriacao, StatusNotificacao status, DateTime? dataEnvio)
        : this(consultaId, tipo, canal, destino, mensagem)
    {
        Id = id;
        DataCriacao = dataCriacao;
        Status = status;
        DataEnvio = dataEnvio;
    }

    public void MarcarComoEnviada()
    {
        if (Status == StatusNotificacao.Enviada)
            throw new InvalidOperationException("A notificação já foi enviada.");
        Status = StatusNotificacao.Enviada;
        DataEnvio = DateTime.Now;
    }
}
