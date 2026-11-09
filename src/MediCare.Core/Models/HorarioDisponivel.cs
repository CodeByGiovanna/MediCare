namespace MediCare.Core.Models;

public class HorarioDisponivel : IEntidade
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Modalidade Modalidade { get; set; }

    // Preenchido somente quando a modalidade é presencial.
    public int? LocalId { get; set; }

    public bool Disponivel { get; private set; } = true;

    public void Reservar()
    {
        if (!Disponivel)
            throw new InvalidOperationException("Este horário já está reservado.");
        Disponivel = false;
    }

    public void Liberar() => Disponivel = true;

    public bool ConflitaCom(HorarioDisponivel outro) =>
        MedicoId == outro.MedicoId &&
        Data == outro.Data &&
        HoraInicio < outro.HoraFim &&
        outro.HoraInicio < HoraFim;

    internal void Restaurar(bool disponivel) => Disponivel = disponivel;
}
