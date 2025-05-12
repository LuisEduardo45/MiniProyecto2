using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProyecto2.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MvcTemplate.Models;    
using MvcTemplate.Data;


namespace MvcTemplate.Controllers
{
    public class EntradaController : Controller
    {
        private readonly MvcTemplate.Data.ApplicationDbContext _context;

        public EntradaController(MvcTemplate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Entrada
        public async Task<IActionResult> Index()
        {
            var entradas = await _context.Entradas.OrderByDescending(e => e.Fecha).ToListAsync();
            ViewBag.Total = entradas.Sum(e => e.Monto);

            return View(entradas);
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
                entrada.Fecha = DateTime.Now;
                _context.Add(entrada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // GET: Entrada/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entrada = await _context.Entradas
                .FirstOrDefaultAsync(m => m.Id == id);
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
