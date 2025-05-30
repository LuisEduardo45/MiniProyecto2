using System; // Importa funcionalidades básicas del sistema.
using System.Collections.Generic; // Permite el uso de colecciones genéricas.
using System.ComponentModel.DataAnnotations; // Proporciona atributos y utilidades para validación de datos.
using MvcTemplate.Models; // Importa los modelos de la aplicación.
using Xunit; // Framework de pruebas unitarias.

namespace MvcTemplate.Tests.Controllers // Define el espacio de nombres para las pruebas de controladores.
{
    public class GastoControllerTests // Clase de pruebas para el controlador de Gasto.
    {
        // Método auxiliar para validar un modelo usando DataAnnotations.
        private IList<ValidationResult> ValidateModel(Gasto model)
        {
            var validationResults = new List<ValidationResult>(); // Lista para almacenar los resultados de validación.
            var ctx = new ValidationContext(model, serviceProvider: null, items: null); // Crea el contexto de validación.
            Validator.TryValidateObject(model, ctx, validationResults, validateAllProperties: true); // Realiza la validación.
            return validationResults; // Devuelve los resultados de validación.
        }

        [Fact] // Prueba de validación: Descripcion es requerida
        public void Gasto_Descripcion_Requerida()
        {
            // Arrange: crea un gasto sin descripción.
            var gasto = new Gasto
            {
                Descripcion = null,
                Monto = 100,
                Fecha = DateTime.Now,
                CategoriaId = 1
            };

            // Act: valida el modelo.
            var results = ValidateModel(gasto);

            // Assert: verifica que la validación detecta la ausencia de descripción.
            Assert.Contains(results, v => v.ErrorMessage == "La descripción es obligatoria.");
        }

        [Fact] // Prueba de validación: Monto debe ser positivo
        public void Gasto_Monto_DebeSerPositivo()
        {
            // Arrange: crea un gasto con monto negativo.
            var gasto = new Gasto
            {
                Descripcion = "Compra",
                Monto = -50,
                Fecha = DateTime.Now,
                CategoriaId = 1
            };

            // Act: valida el modelo.
            var results = ValidateModel(gasto);

            // Assert: verifica que la validación detecta el monto negativo.
            Assert.Contains(results, v => v.ErrorMessage == "El monto debe ser positivo.");
        }

        [Fact] // Prueba de validación: Fecha es requerida
        public void Gasto_Fecha_Requerida()
        {
            // Arrange: crea un gasto sin fecha.
            var gasto = new Gasto
            {
                Descripcion = "Servicio",
                Monto = 75,
                Fecha = null, // Fecha nula para probar el atributo Required.
                CategoriaId = 1
            };

            // Act: valida el modelo.
            var results = ValidateModel(gasto);

            // Assert: verifica que la validación detecta la ausencia de fecha.
            Assert.Contains(results, v => v.ErrorMessage == "La fecha es obligatoria.");
        }

        [Fact] // Prueba de validación: CategoriaId es requerida (> 0)
        public void Gasto_CategoriaId_Requerida()
        {
            // Arrange: crea un gasto con CategoriaId igual a 0 (no seleccionado).
            var gasto = new Gasto
            {
                Descripcion = "Pago",
                Monto = 50,
                Fecha = DateTime.Now,
                CategoriaId = 0 // 0 indica que no se ha seleccionado una categoría.
            };

            // Act: valida el modelo.
            var results = ValidateModel(gasto);

            // Assert: verifica que la validación detecta la ausencia de categoría.
            Assert.Contains(results, v => v.ErrorMessage == "Debe seleccionar una categoría.");
        }

        [Fact] // Prueba de validación: Modelo válido no debe tener errores
        public void Gasto_Modelo_Valido()
        {
            // Arrange: crea un gasto completamente válido.
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

            // Act: valida el modelo.
            var results = ValidateModel(gasto);

            // Assert: verifica que no hay errores de validación.
            Assert.Empty(results); // No debe haber errores
        }
    }
}
