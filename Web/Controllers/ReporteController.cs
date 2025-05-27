using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Data;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Models.ViewModels;


namespace MvcTemplate.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Obtener total de entradas del mes actual
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var totalEntradas = _context.Entradas
                .Where(e => e.Fecha >= inicioMes && e.Fecha <= hoy)
                .Sum(e => (decimal?)e.Valor) ?? 0;

            var categorias = _context.Categorias
                .Include(c => c.Gastos)
                .Where(c => c.Activa)
                .ToList()
                .Select(c => new ReporteCategoriaViewModel
                {
                    Titulo = c.Titulo,
                    PorcentajeMaximo = c.PorcentajeMaximo,
                    GastoTotal = c.Gastos
                        .Where(g => g.Fecha >= inicioMes && g.Fecha <= hoy)
                        .Sum(g => g.Monto),
                    TopePermitido = totalEntradas * c.PorcentajeMaximo / 100
                }).ToList();

            return View(categorias);
        }
    }
}
