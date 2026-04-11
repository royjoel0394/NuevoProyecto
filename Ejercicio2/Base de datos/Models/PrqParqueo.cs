using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParqueoDatabaseExample.Models
{
    [Table("PRQ_Parqueo")]
    public partial class PrqParqueo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_de_provincia")]
        [StringLength(100)]
        public string NombreDeProvincia { get; set; } = null!;

        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Column("precio_por_hora", TypeName = "decimal(10, 2)")]
        public decimal PrecioPorHora { get; set; }

        [InverseProperty("IdParqueoNavigation")]
        public virtual ICollection<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; } = new List<PrqIngresoAutomovile>();
    }
}