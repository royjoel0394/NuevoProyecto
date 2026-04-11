using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories
{
    public interface IParqueoRepository : IRepository<PrqParqueo, int>
    {
        Task<IReadOnlyList<PrqParqueo>> GetByProvinciaAsync(string provincia, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrqParqueo>> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrqParqueo>> GetByPrecioHoraRangeAsync(decimal minPrecio, decimal maxPrecio, CancellationToken cancellationToken = default);
    }
}
