using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Data;
using System;
using System.Linq;

namespace MvcTemplate.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden acceder
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

        // 🔥 Esta es la acción que necesitas agregar
        public IActionResult Reportes()
        {
            var hoy = DateTime.Today;
            var inicioDeSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            var inicioDeMes = new DateTime(hoy.Year, hoy.Month, 1);

            var gastoTotal = _context.Gastos.Sum(g => (decimal?)g.Monto) ?? 0;
            var gastoMensual = _context.Gastos
                .Where(g => g.Fecha >= inicioDeMes)
                .Sum(g => (decimal?)g.Monto) ?? 0;
            var gastoSemanal = _context.Gastos
                .Where(g => g.Fecha >= inicioDeSemana)
                .Sum(g => (decimal?)g.Monto) ?? 0;
            var gastoDiario = _context.Gastos
                .Where(g => g.Fecha.Date == hoy)
                .Sum(g => (decimal?)g.Monto) ?? 0;

            ViewData["GastoTotal"] = gastoTotal;
            ViewData["GastoMensual"] = gastoMensual;
            ViewData["GastoSemanal"] = gastoSemanal;
            ViewData["GastoDiario"] = gastoDiario;

            return View(); // Esta vista debe estar en Views/Home/Reportes.cshtml
        }
    }
}


