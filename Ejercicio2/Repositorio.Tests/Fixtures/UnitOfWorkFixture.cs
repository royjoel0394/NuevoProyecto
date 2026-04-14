using Ejercicio2.Repositorio.Repositories.Sql;
using ParqueoDatabaseExample.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Repositorio.Tests.Fixtures;

public class UnitOfWorkFixture
{
    public SqlUnitOfWork CreateUnitOfWork()
    {
        var options = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ParqueoDbContext(options);
        var autoRepo = new SqlAutomovilRepository(context);
        var parqueoRepo = new SqlParqueoRepository(context);
        var ingresoRepo = new SqlIngresoAutomovilRepository(context);
        return new SqlUnitOfWork(context, autoRepo, parqueoRepo, ingresoRepo);
    }

    public SqlAutomovilRepository CreateAutomovilRepository()
    {
        var options = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ParqueoDbContext(options);
        return new SqlAutomovilRepository(context);
    }

    public SqlParqueoRepository CreateParqueoRepository()
    {
        var options = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ParqueoDbContext(options);
        return new SqlParqueoRepository(context);
    }

    public SqlIngresoAutomovilRepository CreateIngresoAutomovilRepository()
    {
        var options = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ParqueoDbContext(options);
        return new SqlIngresoAutomovilRepository(context);
    }
}

[CollectionDefinition("UnitOfWork")]
public class UnitOfWorkCollection : ICollectionFixture<UnitOfWorkFixture>
{
}
