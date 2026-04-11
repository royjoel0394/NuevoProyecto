using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories
{
    public interface IAutomovilRepository : IRepository<PrqAutomovile, int>
    {
        Task<IReadOnlyList<PrqAutomovile>> GetByColorAsync(string color, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrqAutomovile>> GetByYearRangeAsync(int yearFrom, int yearTo, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrqAutomovile>> GetByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrqAutomovile>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);
    }
}
