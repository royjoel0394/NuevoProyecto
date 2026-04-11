using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParqueoDatabaseExample.Models
{
    [Table("PRQ_Automoviles")]
    public partial class PrqAutomovile
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("color")]
        [StringLength(50)]
        public string Color { get; set; } = null!;

        [Column("año")]
        public int Año { get; set; }

        [Column("fabricante")]
        [StringLength(100)]
        public string Fabricante { get; set; } = null!;

        [Column("tipo")]
        [StringLength(50)]
        public string Tipo { get; set; } = null!;

        [InverseProperty("IdAutomovilNavigation")]
        public virtual ICollection<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; } = new List<PrqIngresoAutomovile>();
    }
}