using System.IO;
using Ejercicio2.Repositorio.Configuration;
using Ejercicio2.Repositorio.Repositories;
using Ejercicio2.Repositorio.Repositories.Json;
using Ejercicio2.Repositorio.Repositories.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParqueoDatabaseExample.Models;
using Xunit;

namespace Repositorio.Tests.Fixtures;

public class ProviderTestFixture : IDisposable
{
    private IServiceProvider? _sqlProvider;
    private IServiceProvider? _jsonProvider;
    private bool _disposed;

    public IServiceProvider SqlProvider
    {
        get
        {
            if (_sqlProvider == null)
            {
                var services = new ServiceCollection();
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Repository:Provider"] = "SqlServer"
                    })
                    .Build();
                services.AddRepositorio(configuration);
                _sqlProvider = services.BuildServiceProvider();
            }
            return _sqlProvider;
        }
    }

    public IServiceProvider JsonProvider
    {
        get
        {
            if (_jsonProvider == null)
            {
                var services = new ServiceCollection();
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData");
                Directory.CreateDirectory(jsonPath);
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Repository:Provider"] = "Json",
                        ["Repository:JsonFilesPath"] = jsonPath
                    })
                    .Build();
                services.AddRepositorio(configuration);
                _jsonProvider = services.BuildServiceProvider();
            }
            return _jsonProvider;
        }
    }

    public IAutomovilRepository GetSqlAutomovilRepository()
        => SqlProvider.GetRequiredService<IAutomovilRepository>();

    public IAutomovilRepository GetJsonAutomovilRepository()
        => JsonProvider.GetRequiredService<IAutomovilRepository>();

    public IParqueoRepository GetSqlParqueoRepository()
        => SqlProvider.GetRequiredService<IParqueoRepository>();

    public IParqueoRepository GetJsonParqueoRepository()
        => JsonProvider.GetRequiredService<IParqueoRepository>();

    public IUnitOfWork GetSqlUnitOfWork()
        => SqlProvider.GetRequiredService<IUnitOfWork>();

    public IUnitOfWork GetJsonUnitOfWork()
        => JsonProvider.GetRequiredService<IUnitOfWork>();

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_sqlProvider is IDisposable disposableSql)
                disposableSql.Dispose();
            if (_jsonProvider is IDisposable disposableJson)
                disposableJson.Dispose();
            _disposed = true;
        }
    }
}

[CollectionDefinition("ProviderTest")]
public class ProviderTestCollection : ICollectionFixture<ProviderTestFixture>
{
}
