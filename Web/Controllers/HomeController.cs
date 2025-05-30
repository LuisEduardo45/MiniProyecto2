using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTemplate.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden acceder
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var mesActual = DateTime.Now.Month;
            var anioActual = DateTime.Now.Year;

            var totalEntradas = await _context.Entradas
                .Where(e => e.UsuarioId == userId && e.Fecha.Month == mesActual && e.Fecha.Year == anioActual)
                .SumAsync(e => (decimal?)e.Valor) ?? 0;

            var totalGastos = await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha.Month == mesActual && g.Fecha.Year == anioActual)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            var saldo = totalEntradas - totalGastos;

            ViewData["TotalEntradas"] = totalEntradas;
            ViewData["TotalGastos"] = totalGastos;
            ViewData["Saldo"] = saldo;

            var gastosPorCategoria = await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha.Month == mesActual && g.Fecha.Year == anioActual)
                .Include(g => g.Categoria)
                .GroupBy(g => g.Categoria.Titulo)
                .Select(g => new {
                    Categoria = g.Key,
                    Total = g.Sum(x => x.Monto)
                })
                .ToListAsync();

            ViewBag.Categorias = gastosPorCategoria.Select(g => g.Categoria).ToList();
            ViewBag.Montos = gastosPorCategoria.Select(g => g.Total).ToList();

            return View();
        }

        public async Task<IActionResult> Reportes()
        {
            var userId = _userManager.GetUserId(User);
            var hoy = DateTime.Today;
            var inicioDeSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            var inicioDeMes = new DateTime(hoy.Year, hoy.Month, 1);

            var gastoTotal = await _context.Gastos
                .Where(g => g.UsuarioId == userId)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            var gastoMensual = await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha >= inicioDeMes)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            var gastoSemanal = await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha >= inicioDeSemana)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            var gastoDiario = await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha.Date == hoy)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            ViewData["GastoTotal"] = gastoTotal;
            ViewData["GastoMensual"] = gastoMensual;
            ViewData["GastoSemanal"] = gastoSemanal;
            ViewData["GastoDiario"] = gastoDiario;

            return View(); // Asegúrate de que la vista Reportes.cshtml exista
        }
    }
}
