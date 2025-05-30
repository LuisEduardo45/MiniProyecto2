using System; // Importa funcionalidades básicas del sistema.
using System.Linq; // Permite el uso de consultas LINQ sobre colecciones.
using System.Threading.Tasks; // Permite el uso de programación asíncrona.
using Xunit; // Framework de pruebas unitarias.
using Microsoft.AspNetCore.Mvc; // Proporciona clases para controladores y resultados de acción.
using Microsoft.EntityFrameworkCore; // Permite el uso de Entity Framework Core.
using Microsoft.Data.Sqlite; // Permite el uso de bases de datos SQLite.
using MvcTemplate.Controllers; // Importa los controladores de la aplicación.
using MvcTemplate.Models; // Importa los modelos de la aplicación.
using MvcTemplate.Data; // Importa el contexto de datos de la aplicación.

namespace MvcTemplate.Tests.Controllers // Define el espacio de nombres para las pruebas de controladores.
{
    public class HomeControllerTests // Clase de pruebas para el controlador HomeController.
    {
        // Método auxiliar para crear opciones de DbContext usando una conexión SQLite.
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>() // Crea el constructor de opciones.
                .UseSqlite(connection) // Usa la conexión SQLite proporcionada.
                .Options; // Devuelve las opciones configuradas.
        }

        [Fact] // Prueba para Index() -> Debe devolver ViewResult con ViewData y ViewBag correctamente calculados
        public async Task Index_ReturnsViewResult_WithCorrectViewData()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada (importante en SQLite).

                // Simula datos de Entrada y Gasto en el mes actual.
                var now = DateTime.Now;

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada
                {
                    Id = 1,
                    Descripcion = "Entrada Test",
                    Valor = 500,
                    Fecha = now
                });

                // Crea y agrega una categoría de prueba.
                var categoria = new Categoria
                {
                    Id = 1,
                    Titulo = "Categoria Test",
                    PorcentajeMaximo = 50,
                    Activa = true
                };
                context.Categorias.Add(categoria);

                // Agrega un gasto asociado a la categoría.
                context.Gastos.Add(new Gasto
                {
                    Id = 1,
                    Descripcion = "Gasto Test",
                    Monto = 200,
                    Fecha = now,
                    CategoriaId = categoria.Id,
                    Categoria = categoria
                });

                context.SaveChanges(); // Guarda los cambios en la base de datos.

                var controller = new HomeController(context); // Crea una instancia del controlador.

                var result = await controller.Index() as ViewResult; // Llama a la acción Index y castea el resultado.

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.

                // Verifica que las claves esperadas existen en ViewData.
                Assert.True(controller.ViewData.ContainsKey("TotalEntradas"));
                Assert.True(controller.ViewData.ContainsKey("TotalGastos"));
                Assert.True(controller.ViewData.ContainsKey("Saldo"));

                // Obtiene los valores de ViewData y los verifica.
                var totalEntradas = (decimal)controller.ViewData["TotalEntradas"]!;
                var totalGastos = (decimal)controller.ViewData["TotalGastos"]!;
                var saldo = (decimal)controller.ViewData["Saldo"]!;

                Assert.Equal(500, totalEntradas); // Verifica el total de entradas.
                Assert.Equal(200, totalGastos); // Verifica el total de gastos.
                Assert.Equal(300, saldo); // Verifica el saldo calculado.

                // Verifica que ViewBag contiene las colecciones esperadas.
                Assert.NotNull(controller.ViewBag.Categorias);
                Assert.NotNull(controller.ViewBag.Montos);

                // Obtiene las colecciones de ViewBag y verifica que no sean nulas.
                var categorias = controller.ViewBag.Categorias as System.Collections.IEnumerable;
                var montos = controller.ViewBag.Montos as System.Collections.IEnumerable;

                Assert.NotNull(categorias);
                Assert.NotNull(montos);
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Reportes() -> Debe devolver ViewResult con los totales de gasto en ViewData
        public void Reportes_ReturnsViewResult_WithCorrectViewData()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Crea y agrega una categoría válida.
                var categoria = new Categoria
                {
                    Id = 1,
                    Titulo = "Categoria Test",
                    PorcentajeMaximo = 50,
                    Activa = true
                };
                context.Categorias.Add(categoria);
                context.SaveChanges();

                // Calcula fechas relevantes para los reportes.
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                // Agrega gastos asociados a la categoría en diferentes fechas.
                context.Gastos.Add(new Gasto { Id = 1, Descripcion = "Gasto Mes", Monto = 300, Fecha = startOfMonth.AddDays(2), CategoriaId = categoria.Id });
                context.Gastos.Add(new Gasto { Id = 2, Descripcion = "Gasto Semana", Monto = 200, Fecha = startOfWeek.AddDays(1), CategoriaId = categoria.Id });
                context.Gastos.Add(new Gasto { Id = 3, Descripcion = "Gasto Hoy", Monto = 100, Fecha = today, CategoriaId = categoria.Id });

                context.SaveChanges(); // Guarda los cambios en la base de datos.

                var controller = new HomeController(context); // Crea una instancia del controlador.

                var result = controller.Reportes() as ViewResult; // Llama a la acción Reportes y castea el resultado.

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.

                // Verifica que las claves esperadas existen en ViewData.
                Assert.True(controller.ViewData.ContainsKey("GastoTotal"));
                Assert.True(controller.ViewData.ContainsKey("GastoMensual"));
                Assert.True(controller.ViewData.ContainsKey("GastoSemanal"));
                Assert.True(controller.ViewData.ContainsKey("GastoDiario"));

                // Obtiene los valores de ViewData y los verifica.
                var gastoTotal = (decimal)controller.ViewData["GastoTotal"]!;
                var gastoMensual = (decimal)controller.ViewData["GastoMensual"]!;
                var gastoSemanal = (decimal)controller.ViewData["GastoSemanal"]!;
                var gastoDiario = (decimal)controller.ViewData["GastoDiario"]!;

                Assert.Equal(600, gastoTotal); // Verifica el gasto total.
                Assert.True(gastoMensual >= gastoDiario); // El gasto mensual debe ser al menos el diario.
                Assert.True(gastoSemanal >= gastoDiario); // El gasto semanal debe ser al menos el diario.
                Assert.Equal(100, gastoDiario); // Verifica el gasto diario.
            }

            connection.Close(); // Cierra la conexión.
        }

    }
}

