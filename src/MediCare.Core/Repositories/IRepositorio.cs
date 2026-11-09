using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

/// <summary>
/// Operações de persistência usadas pelos serviços. Há duas implementações:
/// CsvRepository (arquivos CSV) e EfRepositorio (EF Core, no projeto MediCare.Data).
/// </summary>
public interface IRepositorio<T> where T : class, IEntidade
{
    IReadOnlyList<T> ListarTodos();
    T? ObterPorId(int id);
    T Adicionar(T item);
    bool Remover(int id);
    void Salvar();
}
