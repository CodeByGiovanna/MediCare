using MediCare.Core.Models;
using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Data;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Tests;

/// <summary>
/// Monta um cenário de teste isolado, numa pasta temporária, com a persistência indicada
/// ("Csv" ou "EfCore"). Os mesmos testes de serviço rodam com as duas implementações.
/// </summary>
public sealed class Ambiente : IDisposable
{
    public static TheoryData<string> Persistencias => new() { "Csv", "EfCore" };

    public static readonly DateOnly Dia = DateOnly.FromDateTime(DateTime.Today.AddDays(10));

    private readonly string _pasta = Path.Combine(Path.GetTempPath(), "medicare-teste-" + Guid.NewGuid());
    private readonly string _persistencia;
    private MediCareDbContext? _contexto;

    public MediCareDados Dados { get; private set; } = null!;
    public ContaService Contas { get; private set; } = null!;
    public NotificacaoService Notificacoes { get; private set; } = null!;
    public AgendamentoService Agendamento { get; private set; } = null!;
    public DisponibilidadeService Disponibilidade { get; private set; } = null!;
    public AtendimentoService Atendimento { get; private set; } = null!;

    // Cenário: dois médicos, dois pacientes e três horários livres.
    public Medico MedicoA { get; private set; } = null!;
    public Medico MedicoB { get; private set; } = null!;
    public Paciente PacienteEmail { get; private set; } = null!;
    public Paciente PacienteSms { get; private set; } = null!;
    public LocalAtendimento LocalA { get; private set; } = null!;
    public HorarioDisponivel PresencialA { get; private set; } = null!;
    public HorarioDisponivel TeleconsultaA { get; private set; } = null!;
    public HorarioDisponivel PresencialB { get; private set; } = null!;

    public Ambiente(string persistencia)
    {
        _persistencia = persistencia;
        Directory.CreateDirectory(_pasta);
        Abrir();
        Popular();
    }

    /// <summary>
    /// Descarta os objetos em memória e lê tudo de novo do armazenamento,
    /// para confirmar que as alterações foram de fato gravadas.
    /// </summary>
    public void Reabrir() => Abrir();

    private void Abrir()
    {
        if (_persistencia == "Csv")
        {
            Dados = MediCareDados.DeArquivosCsv(_pasta);
        }
        else
        {
            _contexto?.Dispose();
            var opcoes = new DbContextOptionsBuilder<MediCareDbContext>()
                .UseSqlite($"Data Source={Path.Combine(_pasta, "teste.db")}")
                .Options;
            _contexto = new MediCareDbContext(opcoes);
            _contexto.Database.EnsureCreated();
            Dados = DadosEfCore.Criar(_contexto);
        }

        Contas = new ContaService(Dados);
        Notificacoes = new NotificacaoService(Dados);
        Agendamento = new AgendamentoService(Dados, Notificacoes);
        Disponibilidade = new DisponibilidadeService(Dados);
        Atendimento = new AtendimentoService(Dados);
    }

    private void Popular()
    {
        var especialidade = Dados.Especialidades.Adicionar(new Especialidade { Nome = "Cardiologia" });
        Dados.Especialidades.Salvar();

        MedicoA = Contas.CadastrarMedico(
            "Carla Mendes", "carla@teste.com", "senha123", "CRM-RJ 1", "2133331111", especialidade.Id);
        MedicoB = Contas.CadastrarMedico(
            "Daniel Rocha", "daniel@teste.com", "senha123", "CRM-RJ 2", "2133332222", especialidade.Id);
        PacienteEmail = Contas.CadastrarPaciente("Ana Souza", "ana@teste.com", "senha123", "21911111111",
            new DateOnly(1990, 4, 12), CanalNotificacao.Email);
        PacienteSms = Contas.CadastrarPaciente("Bruno Lima", "bruno@teste.com", "senha123", "21922222222",
            new DateOnly(1985, 11, 30), CanalNotificacao.Sms);

        LocalA = Dados.Locais.Adicionar(new LocalAtendimento
            { MedicoId = MedicoA.Id, Nome = "Clínica A", Endereco = "Rua A, 10", Cidade = "Rio de Janeiro" });
        var localB = Dados.Locais.Adicionar(new LocalAtendimento
            { MedicoId = MedicoB.Id, Nome = "Clínica B", Endereco = "Rua B, 20", Cidade = "Rio de Janeiro" });
        Dados.Locais.Salvar();

        PresencialA = Disponibilidade.CadastrarHorario(
            MedicoA.Id, Dia, new(9, 0), new(9, 30), Modalidade.Presencial, LocalA.Id);
        TeleconsultaA = Disponibilidade.CadastrarHorario(
            MedicoA.Id, Dia, new(10, 0), new(10, 30), Modalidade.Teleconsulta, null);
        PresencialB = Disponibilidade.CadastrarHorario(
            MedicoB.Id, Dia, new(9, 0), new(9, 30), Modalidade.Presencial, localB.Id);
    }

    public void Dispose()
    {
        _contexto?.Dispose();
        try
        {
            Directory.Delete(_pasta, recursive: true);
        }
        catch (IOException)
        {
            // O SQLite pode manter o arquivo aberto por alguns instantes; a pasta é temporária.
        }
    }
}
