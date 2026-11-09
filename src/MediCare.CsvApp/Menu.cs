namespace MediCare.CsvApp;

public static class Menu
{
    /// <summary>
    /// Mostra as opções até o usuário escolher 0. Erros de regra de negócio
    /// ou de digitação são exibidos e o menu continua.
    /// </summary>
    public static void Executar(string titulo, params (string Texto, Action Acao)[] opcoes)
    {
        while (true)
        {
            Console.WriteLine($"\n== {titulo} ==");
            for (var i = 0; i < opcoes.Length; i++)
                Console.WriteLine($"{i + 1} - {opcoes[i].Texto}");
            Console.WriteLine("0 - Voltar");

            var texto = Entrada.Ler("Opção");
            if (texto is null or "0")
                return;

            if (!int.TryParse(texto, out var opcao) || opcao < 1 || opcao > opcoes.Length)
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }

            try
            {
                opcoes[opcao - 1].Acao();
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or FormatException)
            {
                Console.WriteLine($"Não foi possível concluir: {ex.Message}");
            }
        }
    }
}
