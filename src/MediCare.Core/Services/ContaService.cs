using MediCare.Core.Models;
using MediCare.Core.Repositories;

namespace MediCare.Core.Services;

/// <summary>
/// Contas de usuário: UC01 (criar conta), UC02 (login), UC08 (gerenciar perfil) e UC12 (gerenciar usuários).
/// </summary>
public class ContaService(MediCareDados dados)
{
    public Paciente CadastrarPaciente(string nome, string email, string senha, string telefone,
        DateOnly dataNascimento, CanalNotificacao canal)
    {
        GarantirEmailLivre(email);

        var paciente = new Paciente
        {
            Nome = nome,
            Email = email,
            Telefone = telefone,
            DataNascimento = dataNascimento,
            CanalPreferido = canal
        };
        paciente.DefinirSenha(senha);

        dados.Pacientes.Adicionar(paciente);
        dados.Pacientes.Salvar();
        return paciente;
    }

    public Medico CadastrarMedico(string nome, string email, string senha, string crm, string telefone,
        int especialidadeId)
    {
        GarantirEmailLivre(email);
        if (dados.Especialidades.ObterPorId(especialidadeId) is null)
            throw new ArgumentException("Especialidade não encontrada.");

        var medico = new Medico
        {
            Nome = nome,
            Email = email,
            Crm = crm,
            Telefone = telefone,
            EspecialidadeId = especialidadeId
        };
        medico.DefinirSenha(senha);

        dados.Medicos.Adicionar(medico);
        dados.Medicos.Salvar();
        return medico;
    }

    public Usuario Autenticar(string email, string senha)
    {
        var usuario = ObterPorEmail(email)
            ?? throw new InvalidOperationException("E-mail ou senha inválidos.");

        if (usuario.Provedor != ProvedorAutenticacao.Local)
            throw new InvalidOperationException($"Esta conta utiliza login pelo {usuario.Provedor}.");
        if (!usuario.VerificarSenha(senha))
            throw new InvalidOperationException("E-mail ou senha inválidos.");
        if (!usuario.Ativo)
            throw new InvalidOperationException("Conta desativada. Procure o administrador.");

        return usuario;
    }

    public void AtualizarContato(Paciente paciente, string telefone, CanalNotificacao canal)
    {
        paciente.Telefone = telefone;
        paciente.CanalPreferido = canal;
        dados.Pacientes.Salvar();
    }

    public Usuario? ObterPorEmail(string email) =>
        dados.TodosOsUsuarios().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Usuario> ListarUsuarios() => dados.TodosOsUsuarios();

    public void AlterarSituacao(string email, bool ativo)
    {
        var usuario = ObterPorEmail(email) ?? throw new InvalidOperationException("Usuário não encontrado.");
        if (ativo) usuario.Ativar();
        else usuario.Desativar();
        dados.SalvarUsuarios();
    }

    private void GarantirEmailLivre(string email)
    {
        if (ObterPorEmail(email) is not null)
            throw new InvalidOperationException("Já existe uma conta com este e-mail.");
    }
}
