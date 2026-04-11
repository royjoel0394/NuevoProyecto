using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public sealed class JsonUnitOfWork : IUnitOfWork
    {
        public JsonUnitOfWork(
            IAutomovilRepository automovilRepository,
            IParqueoRepository parqueoRepository,
            IIngresoAutomovilRepository ingresoAutomovilRepository)
        {
            AutomovilRepository = automovilRepository;
            ParqueoRepository = parqueoRepository;
            IngresoAutomovilRepository = ingresoAutomovilRepository;
        }

        public IAutomovilRepository AutomovilRepository { get; }
        public IParqueoRepository ParqueoRepository { get; }
        public IIngresoAutomovilRepository IngresoAutomovilRepository { get; }

        public void SaveChanges()
        {
            // Las operaciones JSON escriben inmediatamente. Esta implementación es un no-op.
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
