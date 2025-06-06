﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;  // Agregar este using

namespace MvcTemplate.Models
{
    public class Entrada
    {
        public int Id { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser un número positivo.")]
        public decimal Valor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        // Relación con el usuario que creó la entrada
        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        [ValidateNever]   // <-- Aquí
        public ApplicationUser Usuario { get; set; }
    }
}
