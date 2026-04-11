using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParqueoDatabaseExample.Models
{
    [Table("PRQ_IngresoAutomoviles")]
    public partial class PrqIngresoAutomovile
    {
        [Key]
        [Column("consecutivo")]
        public int Consecutivo { get; set; }

        [Column("id_parqueo")]
        public int IdParqueo { get; set; }

        [Column("id_automovil")]
        public int IdAutomovil { get; set; }

        [Column("fecha_hora_entrada")]
        public DateTime FechaHoraEntrada { get; set; }

        [Column("fecha_hora_salida")]
        public DateTime? FechaHoraSalida { get; set; }

        [NotMapped]
        public int? DuracionEstadiaMinutos
        {
            get
            {
                if (FechaHoraSalida == null)
                    return null;

                return (int?)((FechaHoraSalida.Value - FechaHoraEntrada).TotalMinutes);
            }
        }

        [NotMapped]
        public decimal? DuracionEstadiaHoras
        {
            get
            {
                if (FechaHoraSalida == null)
                    return null;

                return Math.Round((decimal)((FechaHoraSalida.Value - FechaHoraEntrada).TotalMinutes / 60.0), 2);
            }
        }

        [NotMapped]
        public decimal? MontoTotalAPagar
        {
            get
            {
                if (FechaHoraSalida == null || IdParqueoNavigation == null)
                    return null;

                var horas = DuracionEstadiaHoras;
                if (horas == null)
                    return null;

                return Math.Round(horas.Value * IdParqueoNavigation.PrecioPorHora, 2);
            }
        }

        [ForeignKey("IdAutomovil")]
        [InverseProperty("PrqIngresoAutomoviles")]
        public virtual PrqAutomovile IdAutomovilNavigation { get; set; } = null!;

        [ForeignKey("IdParqueo")]
        [InverseProperty("PrqIngresoAutomoviles")]
        public virtual PrqParqueo IdParqueoNavigation { get; set; } = null!;
    }
}