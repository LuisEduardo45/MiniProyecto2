using System;
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
    public class HomeControllerTests
    {
        // Helper para crear opciones SQLite
        private DbContextOptions<ApplicationDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithCorrectViewData()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated(); // Importante para SQLite

                // Simulamos datos de Entrada y Gasto en el mes actual
                var now = DateTime.Now;

                context.Entradas.Add(new Entrada
                {
                    Id = 1,
                    Descripcion = "Entrada Test",
                    Valor = 500,
                    Fecha = now
                });

                var categoria = new Categoria
                {
                    Id = 1,
                    Titulo = "Categoria Test",
                    PorcentajeMaximo = 50,
                    Activa = true
                };
                context.Categorias.Add(categoria);

                context.Gastos.Add(new Gasto
                {
                    Id = 1,
                    Descripcion = "Gasto Test",
                    Monto = 200,
                    Fecha = now,
                    CategoriaId = categoria.Id,
                    Categoria = categoria
                });

                context.SaveChanges();

                var controller = new HomeController(context);

                var result = await controller.Index() as ViewResult;

                Assert.NotNull(result);

                Assert.True(controller.ViewData.ContainsKey("TotalEntradas"));
                Assert.True(controller.ViewData.ContainsKey("TotalGastos"));
                Assert.True(controller.ViewData.ContainsKey("Saldo"));

                var totalEntradas = (decimal)controller.ViewData["TotalEntradas"]!;
                var totalGastos = (decimal)controller.ViewData["TotalGastos"]!;
                var saldo = (decimal)controller.ViewData["Saldo"]!;

                Assert.Equal(500, totalEntradas);
                Assert.Equal(200, totalGastos);
                Assert.Equal(300, saldo);

                Assert.NotNull(controller.ViewBag.Categorias);
                Assert.NotNull(controller.ViewBag.Montos);

                var categorias = controller.ViewBag.Categorias as System.Collections.IEnumerable;
                var montos = controller.ViewBag.Montos as System.Collections.IEnumerable;

                Assert.NotNull(categorias);
                Assert.NotNull(montos);
            }

            connection.Close();
        }

        [Fact]
        public void Reportes_ReturnsViewResult_WithCorrectViewData()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = CreateSqliteOptions(connection);

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated(); // Importante para SQLite

                // Crear una categoría válida
                var categoria = new Categoria
                {
                    Id = 1,
                    Titulo = "Categoria Test",
                    PorcentajeMaximo = 50,
                    Activa = true
                };
                context.Categorias.Add(categoria);
                context.SaveChanges();

                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                // Asociar los gastos a la categoría creada
                context.Gastos.Add(new Gasto { Id = 1, Descripcion = "Gasto Mes", Monto = 300, Fecha = startOfMonth.AddDays(2), CategoriaId = categoria.Id });
                context.Gastos.Add(new Gasto { Id = 2, Descripcion = "Gasto Semana", Monto = 200, Fecha = startOfWeek.AddDays(1), CategoriaId = categoria.Id });
                context.Gastos.Add(new Gasto { Id = 3, Descripcion = "Gasto Hoy", Monto = 100, Fecha = today, CategoriaId = categoria.Id });

                context.SaveChanges();

                var controller = new HomeController(context);

                var result = controller.Reportes() as ViewResult;

                Assert.NotNull(result);

                Assert.True(controller.ViewData.ContainsKey("GastoTotal"));
                Assert.True(controller.ViewData.ContainsKey("GastoMensual"));
                Assert.True(controller.ViewData.ContainsKey("GastoSemanal"));
                Assert.True(controller.ViewData.ContainsKey("GastoDiario"));

                var gastoTotal = (decimal)controller.ViewData["GastoTotal"]!;
                var gastoMensual = (decimal)controller.ViewData["GastoMensual"]!;
                var gastoSemanal = (decimal)controller.ViewData["GastoSemanal"]!;
                var gastoDiario = (decimal)controller.ViewData["GastoDiario"]!;

                Assert.Equal(600, gastoTotal);
                Assert.True(gastoMensual >= gastoDiario);
                Assert.True(gastoSemanal >= gastoDiario);
                Assert.Equal(100, gastoDiario);
            }

            connection.Close();
        }

    }
}
