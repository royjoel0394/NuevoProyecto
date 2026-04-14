using System.IO;
using Ejercicio2.Repositorio.Configuration;
using Ejercicio2.Repositorio.Repositories;
using Ejercicio2.Repositorio.Repositories.Json;
using Ejercicio2.Repositorio.Repositories.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ParqueoDatabaseExample.Models;

namespace Repositorio.Tests.Unit;

public class ProviderSwitchingTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private IServiceProvider? _sqlProvider;
    private IServiceProvider? _jsonProvider;
    private readonly string _jsonPath;

    public ProviderSwitchingTests(ITestOutputHelper output)
    {
        _output = output;
        _jsonPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_jsonPath);
    }

    private IServiceProvider CreateSqlProvider()
    {
        var services = new ServiceCollection();
        var inMemoryOptions = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        services.AddScoped(sp => new ParqueoDbContext(inMemoryOptions));
        services.AddScoped<IAutomovilRepository, SqlAutomovilRepository>();
        services.AddScoped<IParqueoRepository, SqlParqueoRepository>();
        services.AddScoped<IIngresoAutomovilRepository, SqlIngresoAutomovilRepository>();
        services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
        
        return services.BuildServiceProvider();
    }

    private IServiceProvider CreateJsonProvider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        services.AddRepositorio(configuration);
        return services.BuildServiceProvider();
    }

    #region Configuration Tests

    [Fact(Skip = "Requires JSON files with proper permissions")]
    public void Configuration_CanSwitchBetweenSqlAndJson()
    {
        var sqlServices = new ServiceCollection();
        var sqlConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "SqlServer"
            })
            .Build();
        sqlServices.AddRepositorio(sqlConfig);
        var sqlProvider = sqlServices.BuildServiceProvider();

        var jsonServices = new ServiceCollection();
        var jsonConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        jsonServices.AddRepositorio(jsonConfig);
        var jsonProvider = jsonServices.BuildServiceProvider();

        var sqlRepo = sqlProvider.GetRequiredService<IAutomovilRepository>();
        var jsonRepo = jsonProvider.GetRequiredService<IAutomovilRepository>();

        Assert.IsType<SqlAutomovilRepository>(sqlRepo);
        Assert.IsType<JsonAutomovilRepository>(jsonRepo);
    }

    [Fact]
    public void SqlProvider_RegistersCorrectRepositoryTypes()
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

        var autoRepo = _sqlProvider.GetRequiredService<IAutomovilRepository>();
        var parqueoRepo = _sqlProvider.GetRequiredService<IParqueoRepository>();
        var ingresoRepo = _sqlProvider.GetRequiredService<IIngresoAutomovilRepository>();
        var unitOfWork = _sqlProvider.GetRequiredService<IUnitOfWork>();

        Assert.IsType<SqlAutomovilRepository>(autoRepo);
        Assert.IsType<SqlParqueoRepository>(parqueoRepo);
        Assert.IsType<SqlIngresoAutomovilRepository>(ingresoRepo);
        Assert.IsType<SqlUnitOfWork>(unitOfWork);
    }

    [Fact(Skip = "Requires JSON files with proper permissions")]
    public void JsonProvider_RegistersCorrectRepositoryTypes()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        services.AddRepositorio(configuration);
        _jsonProvider = services.BuildServiceProvider();

        var autoRepo = _jsonProvider.GetRequiredService<IAutomovilRepository>();
        var parqueoRepo = _jsonProvider.GetRequiredService<IParqueoRepository>();
        var ingresoRepo = _jsonProvider.GetRequiredService<IIngresoAutomovilRepository>();
        var unitOfWork = _jsonProvider.GetRequiredService<IUnitOfWork>();

        Assert.IsType<JsonAutomovilRepository>(autoRepo);
        Assert.IsType<JsonParqueoRepository>(parqueoRepo);
        Assert.IsType<JsonIngresoAutomovilRepository>(ingresoRepo);
        Assert.IsType<JsonUnitOfWork>(unitOfWork);
    }

    #endregion

    #region Repository Interface Tests

    [Fact(Skip = "Requires JSON files with proper permissions")]
    public void BothProviders_ImplementSameRepositoryInterface()
    {
        var sqlServices = new ServiceCollection();
        var sqlConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "SqlServer"
            })
            .Build();
        sqlServices.AddRepositorio(sqlConfig);
        var sqlProvider = sqlServices.BuildServiceProvider();

        var jsonServices = new ServiceCollection();
        var jsonConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        jsonServices.AddRepositorio(jsonConfig);
        var jsonProvider = jsonServices.BuildServiceProvider();

        var sqlRepo = sqlProvider.GetRequiredService<IAutomovilRepository>();
        var jsonRepo = jsonProvider.GetRequiredService<IAutomovilRepository>();

        Assert.Equal(typeof(IAutomovilRepository), sqlRepo.GetType().GetInterface("IAutomovilRepository"));
        Assert.Equal(typeof(IAutomovilRepository), jsonRepo.GetType().GetInterface("IAutomovilRepository"));
    }

    #endregion

    #region UnitOfWork Tests

    [Fact]
    public void SqlProvider_ProvidesCorrectUnitOfWork()
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

        var unitOfWork = _sqlProvider.GetRequiredService<IUnitOfWork>();
        
        Assert.NotNull(unitOfWork.AutomovilRepository);
        Assert.NotNull(unitOfWork.ParqueoRepository);
        Assert.NotNull(unitOfWork.IngresoAutomovilRepository);
    }

    [Fact(Skip = "Requires JSON files with proper permissions")]
    public void JsonProvider_ProvidesCorrectUnitOfWork()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        services.AddRepositorio(configuration);
        _jsonProvider = services.BuildServiceProvider();

        var unitOfWork = _jsonProvider.GetRequiredService<IUnitOfWork>();
        
        Assert.NotNull(unitOfWork.AutomovilRepository);
        Assert.NotNull(unitOfWork.ParqueoRepository);
        Assert.NotNull(unitOfWork.IngresoAutomovilRepository);
    }

    #endregion

    #region Parity Tests

    [Fact(Skip = "Requires JSON files with proper permissions")]
    public async Task SqlAndJson_GetAll_ReturnsSameInterfaceResult()
    {
        var inMemoryOptions = new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var sqlContext = new ParqueoDbContext(inMemoryOptions);
        var sqlRepo = new SqlAutomovilRepository(sqlContext);
        
        var jsonServices = new ServiceCollection();
        var jsonConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repository:Provider"] = "Json",
                ["Repository:JsonFilesPath"] = _jsonPath
            })
            .Build();
        jsonServices.AddRepositorio(jsonConfig);
        var jsonProvider = jsonServices.BuildServiceProvider();
        var jsonRepo = jsonProvider.GetRequiredService<IAutomovilRepository>();

        var sqlResult = await sqlRepo.GetAllAsync();
        var jsonResult = await jsonRepo.GetAllAsync();

        Assert.Equal(typeof(IReadOnlyList<PrqAutomovile>), sqlResult.GetType());
        Assert.Equal(typeof(IReadOnlyList<PrqAutomovile>), jsonResult.GetType());
    }

    #endregion

    public void Dispose()
    {
        if (_sqlProvider is IDisposable disposableSql)
            disposableSql.Dispose();
        if (_jsonProvider is IDisposable disposableJson)
            disposableJson.Dispose();
        
        try
        {
            if (Directory.Exists(_jsonPath))
                Directory.Delete(_jsonPath, true);
        }
        catch
        {
        }
    }
}
