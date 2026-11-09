using MediCare.Core.Models;
using MediCare.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Data;

/// <summary>
/// Implementação de IRepositorio com EF Core. As entidades ficam rastreadas pelo contexto,
/// então as alterações feitas pelos serviços são gravadas em Salvar().
/// </summary>
public class EfRepositorio<T>(MediCareDbContext contexto) : IRepositorio<T> where T : class, IEntidade
{
    private readonly DbSet<T> _conjunto = contexto.Set<T>();

    public IReadOnlyList<T> ListarTodos()
    {
        // Local inclui entidades adicionadas e ainda não gravadas, como no repositório CSV.
        _conjunto.Load();
        return _conjunto.Local.OrderBy(i => i.Id).ToList();
    }

    public T? ObterPorId(int id) => _conjunto.Find(id);

    public T Adicionar(T item)
    {
        _conjunto.Add(item);
        return item;
    }

    public bool Remover(int id)
    {
        var item = ObterPorId(id);
        if (item is null)
            return false;
        _conjunto.Remove(item);
        return true;
    }

    public void Salvar() => contexto.SaveChanges();
}
