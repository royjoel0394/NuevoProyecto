using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio2.Repositorio.Repositories
{
    public interface IRepository<T, TKey>
        where T : class
    {
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        void Insert(T entity);
        Task InsertAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        void Delete(TKey id);
        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        bool Exists(TKey id);
        Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
