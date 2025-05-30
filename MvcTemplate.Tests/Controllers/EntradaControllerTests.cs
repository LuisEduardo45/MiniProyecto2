using System; // Importa funcionalidades básicas del sistema.
using System.Collections.Generic; // Permite el uso de colecciones genéricas.
using System.Linq; // Proporciona métodos LINQ para consultas sobre colecciones.
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
    public class EntradaControllerTests // Clase de pruebas para el controlador EntradaController.
    {
        // Método privado para crear opciones de DbContext usando una conexión SQLite.
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            // Crea y configura las opciones para usar SQLite.
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection) // Usa la conexión SQLite proporcionada.
                .Options; // Devuelve las opciones configuradas.
        }

        [Fact] // Prueba para Index() -> Debe devolver ViewResult con la lista de entradas
        public async Task Index_ReturnsViewResult_WithEntradaList()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios en la base de datos.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = await controller.Index() as ViewResult; // Llama a la acción Index y castea el resultado.

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                var model = Assert.IsAssignableFrom<IEnumerable<Entrada>>(result!.Model); // Verifica el tipo del modelo.
                Assert.Single(model); // Verifica que solo haya una entrada en el modelo.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Create() (GET) -> Debe devolver la vista de creación
        public void Create_Get_ReturnsView()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = controller.Create(); // Llama a la acción Create (GET).

                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Create() (POST) con datos válidos -> Debe redirigir a Index + crear gasto automático
        public async Task Create_Post_EntradaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría de prueba.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Cat1", PorcentajeMaximo = (int)0.1m, Activa = false });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                // Crea una nueva entrada válida.
                var entrada = new Entrada
                {
                    Descripcion = "Nueva Entrada",
                    Valor = 200,
                    Fecha = DateTime.Now
                };

                var result = await controller.Create(entrada); // Llama a la acción Create (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.
                Assert.Equal(1, context.Entradas.Count()); // Verifica que se guardó una entrada.
                Assert.Equal(1, context.Gastos.Count()); // Verifica la creación automática de gasto.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Edit() (GET) con ID existente -> Debe devolver la vista con la entrada
        public async Task Edit_Get_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = await controller.Edit(1) as ViewResult; // Llama a la acción Edit (GET).

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
                Assert.IsType<Entrada>(result!.Model!); // Verifica que el modelo es de tipo Entrada.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] //Prueba para Edit() (POST) con datos válidos -> Debe redirigir a Index
        public async Task Edit_Post_EntradaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            // PASO 1 — primero agregamos la entrada en un contexto
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada original.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Original", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios.
            }

            // PASO 2 — ahora abrimos otro contexto para el test real
            using (var context2 = new ApplicationDbContext(options))
            {
                var controller = new EntradaController(context2); // Crea una instancia del controlador.

                // Crea una entrada editada válida.
                var entradaEditada = new Entrada
                {
                    Id = 1,
                    Descripcion = "Editada",
                    Valor = 150,
                    Fecha = DateTime.Now
                };

                var result = await controller.Edit(1, entradaEditada); // Llama a la acción Edit (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.

                var entradaActualizada = context2.Entradas.First(); // Obtiene la entrada actualizada.
                Assert.Equal("Editada", entradaActualizada.Descripcion); // Verifica la descripción actualizada.
                Assert.Equal(150, entradaActualizada.Valor); // Verifica el valor actualizado.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Details() con ID existente -> Debe devolver la vista con la entrada
        public async Task Details_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = await controller.Details(1) as ViewResult; // Llama a la acción Details.

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
                Assert.IsType<Entrada>(result!.Model!); // Verifica que el modelo es de tipo Entrada.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Delete() (GET) con ID existente -> Debe devolver la vista de confirmación
        public async Task Delete_Get_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = await controller.Delete(1) as ViewResult; // Llama a la acción Delete (GET).

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
                Assert.IsType<Entrada>(result!.Model!); // Verifica que el modelo es de tipo Entrada.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para DeleteConfirmed() (POST) con ID existente -> Debe eliminar la entrada y redirigir

        public async Task DeleteConfirmed_ExistingId_EliminaYRedirecciona()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una entrada de prueba.
                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new EntradaController(context); // Crea una instancia del controlador.

                var result = await controller.DeleteConfirmed(1); // Llama a la acción DeleteConfirmed (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.
                Assert.Empty(context.Entradas); // Verifica que la lista de entradas está vacía.
            }

            connection.Close(); // Cierra la conexión.
        }
    }
}
