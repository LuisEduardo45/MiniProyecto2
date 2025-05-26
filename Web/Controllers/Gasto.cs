using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    [Authorize]
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
            var gastos = _context.Gastos.Include(g => g.Categoria).ToList();
            return View(gastos);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
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
                // Obtener presupuesto total (sumar todas las entradas)
                decimal totalEntradas = _context.Entradas.Sum(e => e.Valor);

                // Obtener categoría
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId && c.Activa);
                if (categoria == null)
                {
                    ModelState.AddModelError("", "Categoría no válida.");
                    ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                // Sumar gastos actuales en esta categoría
                decimal gastoActual = _context.Gastos
                    .Where(g => g.CategoriaId == gasto.CategoriaId)
                    .Sum(g => g.Monto);

                // Calcular tope máximo
                decimal maximoPermitido = (categoria.PorcentajeMaximo / 100m) * totalEntradas;

                // Validar que no supere el tope
                if (gastoActual + gasto.Monto > maximoPermitido)
                {
                    ModelState.AddModelError("", $"El gasto excede el tope máximo permitido de ${maximoPermitido:C} para la categoría '{categoria.Titulo}'.");
                    ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                _context.Gastos.Add(gasto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

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

                // Obtener presupuesto total
                decimal totalEntradas = _context.Entradas.Sum(e => e.Valor);

                // Obtener categoría
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId && c.Activa);
                if (categoria == null)
                {
                    ModelState.AddModelError("", "Categoría no válida.");
                    ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                // Sumar gastos actuales en esta categoría, excepto el gasto que se está editando
                decimal gastoActual = _context.Gastos
                    .Where(g => g.CategoriaId == gasto.CategoriaId && g.Id != gasto.Id)
                    .Sum(g => g.Monto);

                // Calcular tope máximo
                decimal maximoPermitido = (categoria.PorcentajeMaximo / 100m) * totalEntradas;

                // Validar que no supere el tope
                if (gastoActual + gasto.Monto > maximoPermitido)
                {
                    ModelState.AddModelError("", $"El gasto excede el tope máximo permitido de ${maximoPermitido} para la categoría '{categoria.Titulo}'.");
                    ViewBag.Categorias = _context.Categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                // Actualizar gasto
                gastoExistente.Descripcion = gasto.Descripcion;
                gastoExistente.Monto = gasto.Monto;
                gastoExistente.Fecha = gasto.Fecha;
                gastoExistente.CategoriaId = gasto.CategoriaId;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

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
