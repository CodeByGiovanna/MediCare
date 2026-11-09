using MediCare.Core.Repositories;
using MediCare.Core.Services;
using MediCare.Data;
using MediCare.Web.Infraestrutura;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Pasta dos arquivos CSV, relativa ao projeto. Na persistência com EF Core,
// esses arquivos são usados apenas para preencher o banco na primeira execução.
var pastaDados = CaminhoDoProjeto(builder.Configuration["PastaDados"] ?? "dados");
var usarCsv = builder.Configuration["Persistencia"] == "Csv";

if (usarCsv)
{
    builder.Services.AddSingleton(MediCareDados.DeArquivosCsv(pastaDados));
}
else
{
    var banco = CaminhoDoProjeto(builder.Configuration["BancoDeDados"] ?? "medicare.db");
    builder.Services.AddDbContext<MediCareDbContext>(opcoes => opcoes.UseSqlite($"Data Source={banco}"));
    builder.Services.AddScoped(sp => DadosEfCore.Criar(sp.GetRequiredService<MediCareDbContext>()));
}

builder.Services.AddScoped<NotificacaoService>();
builder.Services.AddScoped<ContaService>();
builder.Services.AddScoped<AgendamentoService>();
builder.Services.AddScoped<DisponibilidadeService>();
builder.Services.AddScoped<AtendimentoService>();
builder.Services.AddScoped<UsuarioLogado>();
builder.Services.AddScoped<Apresentacao>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opcoes =>
    {
        opcoes.LoginPath = "/Conta/Entrar";
        opcoes.LogoutPath = "/Conta/Sair";
        opcoes.AccessDeniedPath = "/Conta/AcessoNegado";
    });

builder.Services.AddAuthorization(opcoes =>
{
    opcoes.AddPolicy(Perfis.Agendamento, p => p.RequireRole(Perfis.Paciente, Perfis.Recepcionista));
    opcoes.AddPolicy(Perfis.Teleconsulta, p => p.RequireRole(Perfis.Paciente, Perfis.Medico));
    opcoes.AddPolicy(Perfis.Paciente, p => p.RequireRole(Perfis.Paciente));
    opcoes.AddPolicy(Perfis.Recepcionista, p => p.RequireRole(Perfis.Recepcionista));
    opcoes.AddPolicy(Perfis.Medico, p => p.RequireRole(Perfis.Medico));
    opcoes.AddPolicy(Perfis.Administrador, p => p.RequireRole(Perfis.Administrador));
});

builder.Services
    .AddRazorPages(opcoes =>
    {
        opcoes.Conventions.AuthorizeFolder("/Agendamento", Perfis.Agendamento);
        opcoes.Conventions.AuthorizeFolder("/Teleconsulta", Perfis.Teleconsulta);
        opcoes.Conventions.AuthorizeFolder("/AreaPaciente", Perfis.Paciente);
        opcoes.Conventions.AuthorizeFolder("/Recepcao", Perfis.Recepcionista);
        opcoes.Conventions.AuthorizeFolder("/AreaMedico", Perfis.Medico);
        opcoes.Conventions.AuthorizeFolder("/Admin", Perfis.Administrador);
    })
    .AddMvcOptions(opcoes => opcoes.Filters.Add<AcessoExclusivoFilter>());

var app = builder.Build();

if (!usarCsv)
{
    using var escopo = app.Services.CreateScope();
    DadosEfCore.PrepararBanco(escopo.ServiceProvider.GetRequiredService<MediCareDbContext>(), pastaDados);
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Erro");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

string CaminhoDoProjeto(string caminho) =>
    Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, caminho));
