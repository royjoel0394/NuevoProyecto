using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio2.Repositorio.Repositories
{
    public interface IUnitOfWork
    {
        IAutomovilRepository AutomovilRepository { get; }
        IParqueoRepository ParqueoRepository { get; }
        IIngresoAutomovilRepository IngresoAutomovilRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
