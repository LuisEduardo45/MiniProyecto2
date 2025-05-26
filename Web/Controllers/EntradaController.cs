using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class EntradaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntradaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Entrada
        public async Task<IActionResult> Index()
        {
            return View(await _context.Entradas.OrderByDescending(e => e.Fecha).ToListAsync());
        }

        // GET: Entrada/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrada/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Entrada entrada)
        {
            if (ModelState.IsValid)
            {
                _context.Entradas.Add(entrada);
                await _context.SaveChangesAsync();

                var categorias = await _context.Categorias
                    .Where(c => !c.Activa)
                    .ToListAsync();

                var totalPorcentaje = categorias.Sum(c => c.PorcentajeMaximo);
                if (totalPorcentaje != 1.0m)
                {
                    ModelState.AddModelError("", "La suma de los porcentajes de categorías debe ser exactamente 100%.");
                    return View(entrada);
                }

                foreach (var categoria in categorias)
                {
                    var montoAsignado = entrada.Valor * categoria.PorcentajeMaximo;

                    var gasto = new Gasto
                    {
                        CategoriaId = categoria.Id,
                        Monto = montoAsignado,
                        Fecha = entrada.Fecha,
                        Descripcion = $"Distribución automática de entrada #{entrada.Id}"
                    };

                    _context.Gastos.Add(gasto);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(entrada);
        }

        // GET: Entrada/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entrada = await _context.Entradas.FindAsync(id);
            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // POST: Entrada/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Entrada entrada)
        {
            if (id != entrada.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Entradas.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(entrada);
        }

        // GET: Entrada/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entrada = await _context.Entradas.FirstOrDefaultAsync(m => m.Id == id);
            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // GET: Entrada/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entrada = await _context.Entradas.FirstOrDefaultAsync(m => m.Id == id);
            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // POST: Entrada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrada = await _context.Entradas.FindAsync(id);
            if (entrada != null)
            {
                _context.Entradas.Remove(entrada);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
