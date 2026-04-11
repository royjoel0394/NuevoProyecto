using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ParqueoDatabaseExample.Models
{
    public partial class ParqueoDbContext : DbContext
    {
        public ParqueoDbContext()
        {
        }

        public ParqueoDbContext(DbContextOptions<ParqueoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PrqAutomovile> PrqAutomoviles { get; set; } = null!;

        public virtual DbSet<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; } = null!;

        public virtual DbSet<PrqParqueo> PrqParqueos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configuración por defecto - usar variables de entorno en producción
                string server = Environment.GetEnvironmentVariable("SQL_SERVER_HOST") ?? "hecferme-sqlserver.database.windows.net";
                string database = Environment.GetEnvironmentVariable("SQL_SERVER_DATABASE") ?? "hecferme-sql-server";
                string userId = Environment.GetEnvironmentVariable("SQL_SERVER_USER") ?? "user01";
                string password = Environment.GetEnvironmentVariable("SQL_SERVER_PASSWORD") ?? "MyVeryStr0ngPassw0rd*";

                string connectionString = $"Server={server},1433;Database={database};User Id={userId};Password={password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrqAutomovile>(entity =>
            {
                entity.Property(e => e.Color).IsUnicode(false);
                entity.Property(e => e.Fabricante).IsUnicode(false);
                entity.Property(e => e.Tipo).IsUnicode(false);
            });

            modelBuilder.Entity<PrqIngresoAutomovile>(entity =>
            {
                entity.HasOne(d => d.IdAutomovilNavigation)
                    .WithMany(p => p.PrqIngresoAutomoviles)
                    .HasForeignKey(d => d.IdAutomovil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IngresoAutomoviles_Automoviles");

                entity.HasOne(d => d.IdParqueoNavigation)
                    .WithMany(p => p.PrqIngresoAutomoviles)
                    .HasForeignKey(d => d.IdParqueo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IngresoAutomoviles_Parqueo");
            });

            modelBuilder.Entity<PrqParqueo>(entity =>
            {
                entity.Property(e => e.Nombre).IsUnicode(false);
                entity.Property(e => e.NombreDeProvincia).IsUnicode(false);
                entity.Property(e => e.PrecioPorHora).HasColumnType("decimal(10, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}