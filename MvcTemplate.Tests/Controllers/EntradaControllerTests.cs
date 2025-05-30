using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MvcTemplate.Controllers;
using MvcTemplate.Models;
using MvcTemplate.Data;

namespace MvcTemplate.Tests.Controllers
{
    public class EntradaControllerTests
    {
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithEntradaList()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var result = await controller.Index() as ViewResult;

                Assert.NotNull(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Entrada>>(result!.Model);
                Assert.Single(model);
            }

            connection.Close();
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                var controller = new EntradaController(context);

                var result = controller.Create();

                Assert.IsType<ViewResult>(result);
            }

            connection.Close();
        }

        [Fact]
        public async Task Create_Post_EntradaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Cat1", PorcentajeMaximo = (int)0.1m, Activa = false });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var entrada = new Entrada
                {
                    Descripcion = "Nueva Entrada",
                    Valor = 200,
                    Fecha = DateTime.Now
                };

                var result = await controller.Create(entrada);

                Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal(1, context.Entradas.Count());
                Assert.Equal(1, context.Gastos.Count()); // Verificamos la creación automática de gasto
            }

            connection.Close();
        }

        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var result = await controller.Edit(1) as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);
                Assert.IsType<Entrada>(result!.Model!);
            }

            connection.Close();
        }

        [Fact]
        public async Task Edit_Post_EntradaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            // PASO 1 — primero agregamos la entrada en un contexto
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Original", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();
            }

            // PASO 2 — ahora abrimos otro contexto para el test real
            using (var context2 = new ApplicationDbContext(options))
            {
                var controller = new EntradaController(context2);

                var entradaEditada = new Entrada
                {
                    Id = 1,
                    Descripcion = "Editada",
                    Valor = 150,
                    Fecha = DateTime.Now
                };

                var result = await controller.Edit(1, entradaEditada);

                Assert.IsType<RedirectToActionResult>(result);

                var entradaActualizada = context2.Entradas.First();
                Assert.Equal("Editada", entradaActualizada.Descripcion);
                Assert.Equal(150, entradaActualizada.Valor);
            }

            connection.Close();
        }


        [Fact]
        public async Task Details_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var result = await controller.Details(1) as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);
                Assert.IsType<Entrada>(result!.Model!);
            }

            connection.Close();
        }

        [Fact]
        public async Task Delete_Get_ExistingId_ReturnsViewWithEntrada()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var result = await controller.Delete(1) as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);
                Assert.IsType<Entrada>(result!.Model!);
            }

            connection.Close();
        }

        [Fact]
        public async Task DeleteConfirmed_ExistingId_EliminaYRedirecciona()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Entradas.Add(new Entrada { Id = 1, Descripcion = "Test Entrada", Valor = 100, Fecha = DateTime.Now });
                context.SaveChanges();

                var controller = new EntradaController(context);

                var result = await controller.DeleteConfirmed(1);

                Assert.IsType<RedirectToActionResult>(result);
                Assert.Empty(context.Entradas);
            }

            connection.Close();
        }
    }
}
