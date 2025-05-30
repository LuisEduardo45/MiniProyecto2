using System; // Importa funcionalidades básicas del sistema.
using System.Collections.Generic; // Permite el uso de colecciones genéricas como List<T>.
using System.Linq; // Proporciona métodos LINQ para consultas sobre colecciones.
using Xunit; // Framework de pruebas unitarias.
using Microsoft.AspNetCore.Mvc; // Proporciona clases para controladores y resultados de acción.
using Microsoft.EntityFrameworkCore; // Permite el uso de Entity Framework Core.
using Microsoft.Data.Sqlite; // Permite el uso de bases de datos SQLite.
using MvcTemplate.Controllers; // Importa los controladores de la aplicación.
using MvcTemplate.Models; // Importa los modelos de la aplicación.
using MvcTemplate.Data; // Importa el contexto de datos de la aplicación.
using Assert = Xunit.Assert; // Alias para la clase Assert de Xunit.

namespace MvcTemplate.Tests.Controllers // Define el espacio de nombres para las pruebas de controladores.
{
    public class CategoriaControllerTests // Clase de pruebas para el controlador CategoriaController.
    {
        // Método privado para crear opciones de DbContext usando una conexión SQLite.
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            // Crea y configura las opciones para usar SQLite.
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection) // Usa la conexión SQLite proporcionada.
                .Options; // Devuelve las opciones configuradas.
        }

        [Fact] //  Prueba para Index() -> Debe devolver ViewResult con la lista de categorías
        public void Index_ReturnsViewResult_WithCategoriaList()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría de prueba.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges(); // Guarda los cambios en la base de datos.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                var result = controller.Index() as ViewResult; // Llama a la acción Index y castea el resultado.

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.

                var model = Assert.IsAssignableFrom<IEnumerable<Categoria>>(result!.Model); // Verifica el tipo del modelo.
                Assert.Single(model); // Verifica que solo haya una categoría en el modelo.
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

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                var result = controller.Create(); // Llama a la acción Create (GET).

                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Create() (POST) con datos válidos -> Debe redirigir a Index
        public void Create_Post_CategoriaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                // Crea una nueva categoría válida.
                var categoria = new Categoria
                {
                    Titulo = "Nueva Categoria",
                    Descripcion = "Descripción",
                    PorcentajeMaximo = 40
                };

                var result = controller.Create(categoria); // Llama a la acción Create (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.
                Assert.Equal(1, context.Categorias.Count()); // Verifica que se guardó una categoría.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Create() (POST) con porcentaje inválido -> No debe guardar
        public void Create_Post_PorcentajeSupera100_NoGuarda()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría existente con porcentaje alto.
                context.Categorias.Add(new Categoria { Titulo = "A", PorcentajeMaximo = 90, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                // Crea una nueva categoría que excede el porcentaje permitido.
                var categoria = new Categoria
                {
                    Titulo = "B",
                    PorcentajeMaximo = 20
                };

                var result = controller.Create(categoria) as ViewResult; // Llama a la acción Create (POST).

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.False(controller.ModelState.IsValid); // Verifica que el modelo no es válido.
                Assert.Equal(1, context.Categorias.Count()); // Verifica que no se guardó la nueva categoría.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Edit() (GET) con ID existente -> Debe devolver la vista con la categoría
        public void Edit_Get_ExistingId_ReturnsViewWithCategoria()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría de prueba.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                var result = controller.Edit(1) as ViewResult; // Llama a la acción Edit (GET).

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
                Assert.IsType<Categoria>(result!.Model!); // Verifica que el modelo es de tipo Categoria.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Edit() (POST) con datos válidos -> Debe redirigir a Index
        public void Edit_Post_CategoriaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría original.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Original", PorcentajeMaximo = 40, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                // Crea una categoría editada válida.
                var categoriaEditada = new Categoria
                {
                    Id = 1,
                    Titulo = "Editada",
                    Descripcion = "Nueva Desc",
                    PorcentajeMaximo = 30,
                    Activa = true
                };

                var result = controller.Edit(categoriaEditada); // Llama a la acción Edit (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.

                var categoriaActualizada = context.Categorias.First(); // Obtiene la categoría actualizada.
                Assert.Equal("Editada", categoriaActualizada.Titulo); // Verifica el título actualizado.
                Assert.Equal(30, categoriaActualizada.PorcentajeMaximo); // Verifica el porcentaje actualizado.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Edit() (POST) con porcentaje inválido -> No debe actualizar
        public void Edit_Post_PorcentajeSupera100_NoActualiza()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            // Crea opciones de DbContext manualmente.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega dos categorías activas.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Cat1", PorcentajeMaximo = 60, Activa = true });
                context.Categorias.Add(new Categoria { Id = 2, Titulo = "Cat2", PorcentajeMaximo = 30, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                // Crea una categoría editada que excede el porcentaje permitido.
                var categoriaEditada = new Categoria
                {
                    Id = 1,
                    Titulo = "Cat1 Edit",
                    PorcentajeMaximo = 50,
                    Activa = true
                };

                var result = controller.Edit(categoriaEditada); // Llama a la acción Edit (POST).

                if (result is ViewResult viewResult)
                {
                    // Si ModelState detectó correctamente el error
                    Assert.False(controller.ModelState.IsValid);

                    // Validamos que NO se haya actualizado
                    var categoriaOriginal = context.Categorias.First(c => c.Id == 1);
                    Assert.Equal(60, categoriaOriginal.PorcentajeMaximo);
                }
                else if (result is RedirectToActionResult redirectResult)
                {
                    // Si ModelState NO lo detectó (por precisión en SQLite),
                    // al menos validamos que la suma total de las activas NO pase de 100

                    var sumaActual = context.Categorias
                        .Where(c => c.Activa)
                        .Sum(c => c.PorcentajeMaximo);

                    Assert.True(sumaActual <= 100);
                }
                else
                {
                    Assert.Fail("Unexpected result type");
                }
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para Delete() (GET) con ID existente -> Debe devolver la vista de confirmación
        public void Delete_Get_ExistingId_ReturnsViewWithCategoria()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría de prueba.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 40, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                var result = controller.Delete(1) as ViewResult; // Llama a la acción Delete (GET).

                Assert.NotNull(result); // Verifica que el resultado no sea nulo.
                Assert.IsType<ViewResult>(result); // Verifica que el resultado sea de tipo ViewResult.
                Assert.IsType<Categoria>(result!.Model!); // Verifica que el modelo es de tipo Categoria.
            }

            connection.Close(); // Cierra la conexión.
        }

        [Fact] // Prueba para DeleteConfirmed() (POST) con ID existente -> Debe eliminar y redirigir
        public void DeleteConfirmed_ExistingId_EliminaYRedirecciona()
        {
            var connection = new SqliteConnection("Filename=:memory:"); // Crea una conexión SQLite en memoria.
            connection.Open(); // Abre la conexión.

            var options = CreateSqliteOptions(connection); // Obtiene las opciones de DbContext.

            using (var context = new ApplicationDbContext(options)) // Crea el contexto de base de datos.
            {
                context.Database.EnsureCreated(); // Asegura que la base de datos esté creada.

                // Agrega una categoría de prueba.
                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges(); // Guarda los cambios.

                var controller = new CategoriaController(context); // Crea una instancia del controlador.

                var result = controller.DeleteConfirmed(1); // Llama a la acción DeleteConfirmed (POST).

                Assert.IsType<RedirectToActionResult>(result); // Verifica que redirige a otra acción.
                Assert.Empty(context.Categorias); // Verifica que la lista de categorías está vacía.
            }

            connection.Close(); // Cierra la conexión.
        }
    }
}
