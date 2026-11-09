using MediCare.Core.Models;
using MediCare.CsvApp;
using MediCare.CsvApp.Telas;

// Pasta com os arquivos CSV: primeiro argumento ou ./dados
var pasta = args.Length > 0 ? args[0] : "dados";

Sistema sistema;
try
{
    sistema = new Sistema(pasta);
}
catch (Exception ex) when (ex is InvalidDataException or IOException)
{
    Console.WriteLine($"Erro ao ler os arquivos CSV: {ex.Message}");
    return 1;
}

Console.WriteLine($"MediCare — dados carregados de '{Path.GetFullPath(pasta)}'");

Menu.Executar("MediCare",
    ("Entrar", Entrar),
    ("Criar conta de paciente", () => TelaCadastro.Paciente(sistema)),
    ("Criar conta de médico", () => TelaCadastro.Medico(sistema)));

return 0;

void Entrar()
{
    var usuario = sistema.Contas.Autenticar(Entrada.LerTexto("E-mail"), Entrada.LerTexto("Senha"));
    Console.WriteLine($"Bem-vindo(a), {usuario.Nome}.");

    switch (usuario)
    {
        case Paciente p: MenuPaciente.Abrir(sistema, p); break;
        case Recepcionista: MenuRecepcionista.Abrir(sistema); break;
        case Medico m: MenuMedico.Abrir(sistema, m); break;
        case Administrador: MenuAdministrador.Abrir(sistema); break;
    }
}
