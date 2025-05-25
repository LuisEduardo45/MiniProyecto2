using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GastoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public IActionResult Index()
        {
            // Incluimos la categoría para mostrar título
            var gastos = _context.Gastos.Include(g => g.Categoria).ToList();
            return View(gastos);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            // Solo categorías activas para seleccionar
            ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Gastos.Add(gasto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            // Si hay error, recarga categorías para el dropdown
            ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            // Recargamos categorías activas para el dropdown
            ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                var gastoExistente = _context.Gastos.FirstOrDefault(g => g.Id == gasto.Id);
                if (gastoExistente == null)
                    return NotFound();

                gastoExistente.Descripcion = gasto.Descripcion;
                gastoExistente.Monto = gasto.Monto;
                gastoExistente.Fecha = gasto.Fecha;
                gastoExistente.CategoriaId = gasto.CategoriaId;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            // Recargamos categorías activas si hay error
            ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // ELIMINAR (GET)
        public IActionResult Delete(int id)
        {
            var gasto = _context.Gastos.Include(g => g.Categoria).FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            _context.Gastos.Remove(gasto);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
