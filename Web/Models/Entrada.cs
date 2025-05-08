using System;
using System.ComponentModel.DataAnnotations;

namespace MiniProyecto2.Web.Models
{
    public class Entrada
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string? Descripcion { get; set; }
    }
}
