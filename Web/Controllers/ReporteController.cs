using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using System;
using System.Linq;

public class ReporteController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReporteController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? categoriaId, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        // Obtener categorías para el filtro
        ViewBag.Categorias = _context.Categorias
            .Where(c => c.Activa)
            .ToList();

        var gastosQuery = _context.Gastos.Include(g => g.Categoria).AsQueryable();

        if (categoriaId.HasValue)
            gastosQuery = gastosQuery.Where(g => g.CategoriaId == categoriaId.Value);

        if (fechaDesde.HasValue)
            gastosQuery = gastosQuery.Where(g => g.Fecha >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            gastosQuery = gastosQuery.Where(g => g.Fecha <= fechaHasta.Value);

        // Agrupar gastos por categoría y sumar montos
        var datosReporte = gastosQuery
            .GroupBy(g => g.Categoria.Titulo)
            .Select(grp => new {
                Categoria = grp.Key,
                Total = grp.Sum(g => g.Monto)
            })
            .ToList();

        // Pasar datos a la vista para gráfico
        ViewBag.CategoriasNombres = datosReporte.Select(d => d.Categoria).ToList();
        ViewBag.Montos = datosReporte.Select(d => d.Total).ToList();

        // Pasar filtros actuales para que se mantengan en el formulario
        ViewBag.FiltroCategoria = categoriaId;
        ViewBag.FiltroFechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
        ViewBag.FiltroFechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

        return View();
    }
}
