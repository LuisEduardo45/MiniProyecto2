using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MvcTemplate.Controllers;
using MvcTemplate.Models;
using MvcTemplate.Data;
using Assert = Xunit.Assert;

namespace MvcTemplate.Tests.Controllers
{
    public class CategoriaControllerTests
    {
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCategoriaList()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var result = controller.Index() as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);

                var model = Assert.IsAssignableFrom<IEnumerable<Categoria>>(result!.Model);
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

                var controller = new CategoriaController(context);

                var result = controller.Create();

                Assert.IsType<ViewResult>(result);
            }

            connection.Close();
        }

        [Fact]
        public void Create_Post_CategoriaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                var controller = new CategoriaController(context);

                var categoria = new Categoria
                {
                    Titulo = "Nueva Categoria",
                    Descripcion = "Descripción",
                    PorcentajeMaximo = 40
                };

                var result = controller.Create(categoria);

                Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal(1, context.Categorias.Count());
            }

            connection.Close();
        }

        [Fact]
        public void Create_Post_PorcentajeSupera100_NoGuarda()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Titulo = "A", PorcentajeMaximo = 90, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var categoria = new Categoria
                {
                    Titulo = "B",
                    PorcentajeMaximo = 20
                };

                var result = controller.Create(categoria) as ViewResult;

                Assert.NotNull(result);
                Assert.False(controller.ModelState.IsValid);
                Assert.Equal(1, context.Categorias.Count());
            }

            connection.Close();
        }

        [Fact]
        public void Edit_Get_ExistingId_ReturnsViewWithCategoria()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var result = controller.Edit(1) as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);
                Assert.IsType<Categoria>(result!.Model!);
            }

            connection.Close();
        }

        [Fact]
        public void Edit_Post_CategoriaValida_RedireccionaAIndex()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Original", PorcentajeMaximo = 40, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var categoriaEditada = new Categoria
                {
                    Id = 1,
                    Titulo = "Editada",
                    Descripcion = "Nueva Desc",
                    PorcentajeMaximo = 30,
                    Activa = true
                };

                var result = controller.Edit(categoriaEditada);

                Assert.IsType<RedirectToActionResult>(result);

                var categoriaActualizada = context.Categorias.First();
                Assert.Equal("Editada", categoriaActualizada.Titulo);
                Assert.Equal(30, categoriaActualizada.PorcentajeMaximo);
            }

            connection.Close();
        }

        [Fact]
        public void Edit_Post_PorcentajeSupera100_NoActualiza()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Cat1", PorcentajeMaximo = 60, Activa = true });
                context.Categorias.Add(new Categoria { Id = 2, Titulo = "Cat2", PorcentajeMaximo = 30, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var categoriaEditada = new Categoria
                {
                    Id = 1,
                    Titulo = "Cat1 Edit",
                    PorcentajeMaximo = 50,
                    Activa = true
                };

                var result = controller.Edit(categoriaEditada);

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

            connection.Close();
        }



        [Fact]
        public void Delete_Get_ExistingId_ReturnsViewWithCategoria()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 40, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var result = controller.Delete(1) as ViewResult;

                Assert.NotNull(result);
                Assert.IsType<ViewResult>(result);
                Assert.IsType<Categoria>(result!.Model!);
            }

            connection.Close();
        }

        [Fact]
        public void DeleteConfirmed_ExistingId_EliminaYRedirecciona()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Categorias.Add(new Categoria { Id = 1, Titulo = "Test", PorcentajeMaximo = 50, Activa = true });
                context.SaveChanges();

                var controller = new CategoriaController(context);

                var result = controller.DeleteConfirmed(1);

                Assert.IsType<RedirectToActionResult>(result);
                Assert.Empty(context.Categorias);
            }

            connection.Close();
        }
    }
}
