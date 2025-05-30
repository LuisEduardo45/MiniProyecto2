using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models.ViewModels;
using System;
using System.Linq;
using ClosedXML.Excel;
using System.IO;
using System.Threading.Tasks;
using MvcTemplate.Models;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReporteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? categoriaId, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var userId = _userManager.GetUserId(User);

            // Obtener categorías activas del usuario para el filtro
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.Activa && c.UsuarioId == userId)
                .ToListAsync();

            var gastosQuery = _context.Gastos
                .Include(g => g.Categoria)
                .Where(g => g.UsuarioId == userId)
                .AsQueryable();

            if (categoriaId.HasValue)
                gastosQuery = gastosQuery.Where(g => g.CategoriaId == categoriaId.Value);

            if (fechaDesde.HasValue)
                gastosQuery = gastosQuery.Where(g => g.Fecha >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                gastosQuery = gastosQuery.Where(g => g.Fecha <= fechaHasta.Value);

            // Agrupar gastos por categoría
            var datosReporte = await gastosQuery
                .GroupBy(g => g.Categoria.Titulo)
                .Select(grp => new
                {
                    Categoria = grp.Key,
                    Total = grp.Sum(g => g.Monto)
                })
                .ToListAsync();

            // Obtener total de entradas del usuario
            var totalEntradas = await _context.Entradas
                .Where(e => e.UsuarioId == userId)
                .SumAsync(e => e.Valor);

            // Obtener porcentajes máximos por categoría (solo del usuario)
            var categorias = await _context.Categorias
                .Where(c => c.Activa && c.UsuarioId == userId)
                .ToDictionaryAsync(c => c.Titulo, c => c.PorcentajeMaximo);

            var detalleGastos = datosReporte.Select(d => new ReporteCategoriaViewModel
            {
                Titulo = d.Categoria,
                GastoTotal = d.Total,
                TopePermitido = categorias.ContainsKey(d.Categoria)
                    ? Math.Round(totalEntradas * categorias[d.Categoria] / 100, 2)
                    : 0
            }).ToList();

            ViewBag.DetalleGastos = detalleGastos;
            ViewBag.CategoriasNombres = datosReporte.Select(d => d.Categoria).ToList();
            ViewBag.Montos = datosReporte.Select(d => d.Total).ToList();

            ViewBag.FiltroCategoria = categoriaId;
            ViewBag.FiltroFechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FiltroFechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            // Gráfico de gastos totales por mes (filtrado por usuario)
            var totalesMensuales = await _context.Gastos
                .Where(g => g.UsuarioId == userId &&
                            (!categoriaId.HasValue || g.CategoriaId == categoriaId) &&
                            (!fechaDesde.HasValue || g.Fecha >= fechaDesde) &&
                            (!fechaHasta.HasValue || g.Fecha <= fechaHasta))
                .GroupBy(g => new { g.Fecha.Year, g.Fecha.Month })
                .Select(grp => new
                {
                    Anio = grp.Key.Year,
                    MesNumero = grp.Key.Month,
                    Total = grp.Sum(g => g.Monto)
                })
                .AsEnumerable()
                .Select(grp => new
                {
                    Mes = $"{grp.MesNumero:D2}/{grp.Anio}",
                    Total = grp.Total
                })
                .OrderBy(grp => grp.Mes)
                .ToListAsync();

            ViewBag.Meses = totalesMensuales.Select(t => t.Mes).ToList();
            ViewBag.MontosMensuales = totalesMensuales.Select(t => t.Total).ToList();

            return View();
        }

        public async Task<IActionResult> ExportarExcel(DateTime? fechaDesde, DateTime? fechaHasta, int? categoriaId)
        {
            var userId = _userManager.GetUserId(User);

            var gastosQuery = _context.Gastos
                .Include(g => g.Categoria)
                .Where(g => g.UsuarioId == userId)
                .AsQueryable();

            if (categoriaId.HasValue)
                gastosQuery = gastosQuery.Where(g => g.CategoriaId == categoriaId.Value);

            if (fechaDesde.HasValue)
                gastosQuery = gastosQuery.Where(g => g.Fecha >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                gastosQuery = gastosQuery.Where(g => g.Fecha <= fechaHasta.Value);

            var datosReporte = await gastosQuery
                .GroupBy(g => g.Categoria.Titulo)
                .Select(grp => new
                {
                    Categoria = grp.Key,
                    Monto = grp.Sum(g => g.Monto)
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Gastos");

            worksheet.Cell(1, 1).Value = "Categoría";
            worksheet.Cell(1, 2).Value = "Monto";
            worksheet.Row(1).Style.Font.Bold = true;

            int fila = 2;
            foreach (var item in datosReporte)
            {
                worksheet.Cell(fila, 1).Value = item.Categoria;
                worksheet.Cell(fila, 2).Value = item.Monto;
                fila++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteGastos.xlsx");
        }
    }
}
