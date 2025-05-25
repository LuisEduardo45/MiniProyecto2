using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Data;
using System;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var mesActual = DateTime.Now.Month;
            var anioActual = DateTime.Now.Year;

            var totalEntradas = _context.Entradas
                .Where(e => e.Fecha.Month == mesActual && e.Fecha.Year == anioActual)
                .Sum(e => (decimal?)e.Valor) ?? 0;

            var totalGastos = _context.Gastos
                .Where(g => g.Fecha.Month == mesActual && g.Fecha.Year == anioActual)
                .Sum(g => (decimal?)g.Monto) ?? 0;

            var saldo = totalEntradas - totalGastos;

            ViewData["TotalEntradas"] = totalEntradas;
            ViewData["TotalGastos"] = totalGastos;
            ViewData["Saldo"] = saldo;

            return View();
        }
    }
}

