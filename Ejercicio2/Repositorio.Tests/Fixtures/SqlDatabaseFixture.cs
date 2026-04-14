using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;
using Xunit;

namespace Repositorio.Tests.Fixtures;

public class SqlDatabaseFixture : IDisposable
{
    private bool _disposed;

    public ParqueoDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ParqueoDbContext(options);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

[CollectionDefinition("SqlDatabase")]
public class SqlDatabaseCollection : ICollectionFixture<SqlDatabaseFixture>
{
}
