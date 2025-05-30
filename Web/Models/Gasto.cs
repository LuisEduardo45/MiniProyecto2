using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MvcTemplate.Models
{
    public class Gasto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser positivo.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime? Fecha { get; set; }

        [CategoriaIdRequerida]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }


        // Relación con la categoría
        [ForeignKey("CategoriaId")]
        [ValidateNever] // ← Agrega esto
        public Categoria? Categoria { get; set; }

    }

    internal class CategoriaIdRequeridaAttribute : ValidationAttribute
    {
        public CategoriaIdRequeridaAttribute() : base("Debe seleccionar una categoría.") { }

        public override bool IsValid(object? value)
        {
            if (value is int id)
                return id > 0;
            return false;
        }
    }

}


