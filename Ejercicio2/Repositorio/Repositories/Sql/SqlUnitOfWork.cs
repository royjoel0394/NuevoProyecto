using System.Threading;
using System.Threading.Tasks;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlUnitOfWork : IUnitOfWork
    {
        private readonly ParqueoDbContext _context;

        public SqlUnitOfWork(
            ParqueoDbContext context,
            IAutomovilRepository automovilRepository,
            IParqueoRepository parqueoRepository,
            IIngresoAutomovilRepository ingresoAutomovilRepository)
        {
            _context = context;
            AutomovilRepository = automovilRepository;
            ParqueoRepository = parqueoRepository;
            IngresoAutomovilRepository = ingresoAutomovilRepository;
        }

        public IAutomovilRepository AutomovilRepository { get; }
        public IParqueoRepository ParqueoRepository { get; }
        public IIngresoAutomovilRepository IngresoAutomovilRepository { get; }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
