using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Dtos;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories
{
    public interface IIngresoAutomovilRepository : IRepository<PrqIngresoAutomovile, int>
    {
        Task<decimal?> GetPrecioPorHoraAsync(int parqueoId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<IngresoAutomovilDetalleDto>> GetIngresosPorTipoAutomovilAsync(
            string tipoAutomovil,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<IngresoPorProvinciaDto>> GetIngresosPorProvinciaAsync(
            string provincia,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default);
    }
}
