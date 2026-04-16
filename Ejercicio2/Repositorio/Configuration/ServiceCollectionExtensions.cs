using System;
using System.IO;
using Ejercicio2.Repositorio.Repositories;
using Ejercicio2.Repositorio.Repositories.Json;
using Ejercicio2.Repositorio.Repositories.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositorio(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RepositoryOptions>(configuration.GetSection("Repository"));
            var options = configuration.GetSection("Repository").Get<RepositoryOptions>() ?? new RepositoryOptions();

            if (options.Provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<ParqueoDbContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("ParqueoDb")!));

                // Usar repositorios con Stored Procedures si esta opcion esta habilitada
                if (options.UseStoredProcedures)
                {
                    services.AddScoped<IAutomovilRepository, SqlAutomovilRepositorySp>();
                    services.AddScoped<IParqueoRepository, SqlParqueoRepositorySp>();
                    services.AddScoped<IIngresoAutomovilRepository, SqlIngresoAutomovilRepositorySp>();
                }
                else
                {
                    services.AddScoped<IAutomovilRepository, SqlAutomovilRepository>();
                    services.AddScoped<IParqueoRepository, SqlParqueoRepository>();
                    services.AddScoped<IIngresoAutomovilRepository, SqlIngresoAutomovilRepository>();
                }
                services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
            }
            else
            {
                var basePath = ResolveJsonBasePath(options.JsonFilesPath);
                services.AddSingleton<IJsonFileProvider>(new JsonFileProvider(basePath, options));

                services.AddScoped<IAutomovilRepository, JsonAutomovilRepository>();
                services.AddScoped<IParqueoRepository, JsonParqueoRepository>();
                services.AddScoped<IIngresoAutomovilRepository, JsonIngresoAutomovilRepository>();
                services.AddScoped<IUnitOfWork, JsonUnitOfWork>();
            }

            return services;
        }

        private static string ResolveJsonBasePath(string jsonFilesPath)
        {
            var candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, jsonFilesPath));
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current.Parent != null)
            {
                current = current.Parent;
                candidate = Path.GetFullPath(Path.Combine(current.FullName, jsonFilesPath));
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new DirectoryNotFoundException($"No se pudo resolver la ruta JSON: {jsonFilesPath}");
        }
    }
}
