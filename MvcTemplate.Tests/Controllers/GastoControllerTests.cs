using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MvcTemplate.Models;
using Xunit;

namespace MvcTemplate.Tests.Controllers
{
    public class GastoControllerTests
    {
        // Helper para validar un modelo
        private IList<ValidationResult> ValidateModel(Gasto model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, ctx, validationResults, validateAllProperties: true);
            return validationResults;
        }

        [Fact]
        public void Gasto_Descripcion_Requerida()
        {
            // Arrange
            var gasto = new Gasto
            {
                Descripcion = null,
                Monto = 100,
                Fecha = DateTime.Now,
                CategoriaId = 1
            };

            // Act
            var results = ValidateModel(gasto);

            // Assert
            Assert.Contains(results, v => v.ErrorMessage == "La descripción es obligatoria.");
        }

        [Fact]
        public void Gasto_Monto_DebeSerPositivo()
        {
            // Arrange
            var gasto = new Gasto
            {
                Descripcion = "Compra",
                Monto = -50,
                Fecha = DateTime.Now,
                CategoriaId = 1
            };

            // Act
            var results = ValidateModel(gasto);

            // Assert
            Assert.Contains(results, v => v.ErrorMessage == "El monto debe ser positivo.");
        }
        [Fact]
        public void Gasto_Fecha_Requerida()
        {
            // Arrange
            var gasto = new Gasto
            {
                Descripcion = "Servicio",
                Monto = 75,
                Fecha = null, // Ahora null, para que Required funcione
                CategoriaId = 1
            };

            // Act
            var results = ValidateModel(gasto);

            // Assert
            Assert.Contains(results, v => v.ErrorMessage == "La fecha es obligatoria.");
        }




        [Fact]
        public void Gasto_CategoriaId_Requerida()
        {
            // Arrange
            var gasto = new Gasto
            {
                Descripcion = "Pago",
                Monto = 50,
                Fecha = DateTime.Now,
                CategoriaId = 0 // Usar 0 como valor predeterminado para indicar que no se ha seleccionado una categoría
            };

            // Act
            var results = ValidateModel(gasto);

            // Assert
            Assert.Contains(results, v => v.ErrorMessage == "Debe seleccionar una categoría.");
        }




        [Fact]
        public void Gasto_Modelo_Valido()
        {
            // Arrange
            var gasto = new Gasto
            {
                Descripcion = "Transporte",
                Monto = 25,
                Fecha = DateTime.Now,
                CategoriaId = 2,
                Categoria = new Categoria
                {
                    Id = 2,
                    Titulo = "Movilidad",
                    PorcentajeMaximo = 30,
                    Activa = true
                }
            };

            // Act
            var results = ValidateModel(gasto);

            // Assert
            Assert.Empty(results); // No debe haber errores
        }
    }
}
