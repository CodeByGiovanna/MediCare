using MediCare.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Data;

/// <summary>
/// Mapeamento das entidades do modelo para o banco SQLite.
/// Cada tipo de usuário tem a sua própria tabela; as associações do diagrama de classes
/// viram chaves estrangeiras.
/// </summary>
public class MediCareDbContext(DbContextOptions<MediCareDbContext> options) : DbContext(options)
{
    public DbSet<Especialidade> Especialidades => Set<Especialidade>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Recepcionista> Recepcionistas => Set<Recepcionista>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<LocalAtendimento> Locais => Set<LocalAtendimento>();
    public DbSet<HorarioDisponivel> Horarios => Set<HorarioDisponivel>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Especialidade>().ToTable("Especialidades");
        modelBuilder.Entity<Paciente>().ToTable("Pacientes");
        modelBuilder.Entity<Recepcionista>().ToTable("Recepcionistas");
        modelBuilder.Entity<Administrador>().ToTable("Administradores");

        modelBuilder.Entity<Medico>().ToTable("Medicos")
            .HasOne<Especialidade>().WithMany().HasForeignKey(m => m.EspecialidadeId);

        modelBuilder.Entity<LocalAtendimento>().ToTable("LocaisAtendimento")
            .HasOne<Medico>().WithMany().HasForeignKey(l => l.MedicoId);

        var horario = modelBuilder.Entity<HorarioDisponivel>().ToTable("Horarios");
        horario.HasOne<Medico>().WithMany().HasForeignKey(h => h.MedicoId);
        horario.HasOne<LocalAtendimento>().WithMany().HasForeignKey(h => h.LocalId);

        var consulta = modelBuilder.Entity<Consulta>().ToTable("Consultas");
        consulta.HasOne<Paciente>().WithMany().HasForeignKey(c => c.PacienteId);
        consulta.HasOne<HorarioDisponivel>().WithMany().HasForeignKey(c => c.HorarioId);

        modelBuilder.Entity<Notificacao>().ToTable("Notificacoes")
            .HasOne<Consulta>().WithMany().HasForeignKey(n => n.ConsultaId);
    }
}
