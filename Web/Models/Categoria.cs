using System.ComponentModel.DataAnnotations;

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

        public decimal TopeMaximo { get; set; }

        public bool Activa { get; set; }

    }
}
