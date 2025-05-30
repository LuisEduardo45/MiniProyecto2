using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MvcTemplate.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100)]
        public string Titulo { get; set; }

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El porcentaje máximo es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100.")]
        public int PorcentajeMaximo { get; set; }

        public bool Activa { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        [ValidateNever]
        public ApplicationUser Usuario { get; set; }

        public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
    }
}