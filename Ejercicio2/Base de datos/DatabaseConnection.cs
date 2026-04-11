using System;
using System.Data.SqlClient;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace ParqueoDatabaseExample
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Método seguro para obtener la cadena de conexión desde variables de entorno
        /// </summary>
        public string GetConnectionStringFromEnvironment()
        {
            // Obtener credenciales desde variables de entorno (recomendado)
            string server = Environment.GetEnvironmentVariable("SQL_SERVER_HOST") ?? "hecferme-sqlserver.database.windows.net";
            string database = Environment.GetEnvironmentVariable("SQL_SERVER_DATABASE") ?? "hecferme-sql-server";
            string userId = Environment.GetEnvironmentVariable("SQL_SERVER_USER") ?? "user01";
            string password = Environment.GetEnvironmentVariable("SQL_SERVER_PASSWORD");

            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("La contraseña no está configurada en las variables de entorno.");
            }

            return $"Server={server},1433;Database={database};User Id={userId};Password={password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        /// <summary>
        /// Método seguro para obtener la cadena de conexión desde Azure Key Vault
        /// </summary>
        public async Task<string> GetConnectionStringFromKeyVaultAsync()
        {
            string vaultUri = _configuration["AzureKeyVault:VaultUri"];
            string secretName = _configuration["AzureKeyVault:SecretName"];

            if (string.IsNullOrEmpty(vaultUri) || string.IsNullOrEmpty(secretName))
            {
                throw new InvalidOperationException("Configuración de Azure Key Vault no encontrada.");
            }

            var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);

            return secret.Value;
        }

        /// <summary>
        /// Método para probar la conexión a la base de datos
        /// </summary>
        public async Task TestConnectionAsync(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Conexión exitosa a SQL Server en la nube");

                    // Ejemplo de consulta simple
                    string query = "SELECT COUNT(*) FROM PRQ_Automoviles";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        object? result = await command.ExecuteScalarAsync();
                        int count = result != null ? Convert.ToInt32(result) : 0;
                        Console.WriteLine($"📊 Total de automóviles registrados: {count}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error de conexión: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Método para consultar datos de las tablas del parqueo
        /// </summary>
        public async Task ConsultarDatosParqueoAsync(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Consulta de automóviles
                Console.WriteLine("\n🚗 AUTOMÓVILES REGISTRADOS:");
                string queryAutos = "SELECT id, fabricante, tipo, color, año FROM PRQ_Automoviles ORDER BY id";
                using (SqlCommand command = new SqlCommand(queryAutos, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, {reader["fabricante"]} {reader["tipo"]} {reader["color"]} ({reader["año"]})");
                    }
                }

                // Consulta de parqueos
                Console.WriteLine("\n🏢 PARQUEOS DISPONIBLES:");
                string queryParqueos = "SELECT id, nombre, nombre_de_provincia, precio_por_hora FROM PRQ_Parqueo ORDER BY id";
                using (SqlCommand command = new SqlCommand(queryParqueos, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, {reader["nombre"]} ({reader["nombre_de_provincia"]}) - ₡{reader["precio_por_hora"]}/hora");
                    }
                }

                // Consulta de ingresos activos (sin salida)
                Console.WriteLine("\n🔄 INGRESOS ACTIVOS (AUTOMÓVILES EN PARQUEO):");
                string queryIngresos = @"
                    SELECT i.consecutivo, a.fabricante, a.tipo, p.nombre as parqueo,
                           i.fecha_hora_entrada
                    FROM PRQ_IngresoAutomoviles i
                    INNER JOIN PRQ_Automoviles a ON i.id_automovil = a.id
                    INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
                    WHERE i.fecha_hora_salida IS NULL
                    ORDER BY i.fecha_hora_entrada DESC";
                using (SqlCommand command = new SqlCommand(queryIngresos, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"#{reader["consecutivo"]}: {reader["fabricante"]} {reader["tipo"]} en {reader["parqueo"]} desde {reader["fecha_hora_entrada"]}");
                    }
                }
            }
        }
    }
}