using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using MvcTemplate.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public IActionResult Index()
        {
            var categorias = _context.Categorias.ToList();
            return View(categorias);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var totalActual = _context.Categorias
                    .Where(c => c.Activa)
                    .Sum(c => (decimal?)c.PorcentajeMaximo) ?? 0;

                var nuevoTotal = totalActual + categoria.PorcentajeMaximo;

                if (categoria.PorcentajeMaximo > 0 && nuevoTotal > 100)
                {
                    ModelState.AddModelError("PorcentajeMaximo", $"No puedes agregar esta categoría porque el total superaría el 100% (actual: {totalActual}%).");
                    return View(categoria);
                }

                categoria.Activa = true;
                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var categoriaExistente = _context.Categorias.FirstOrDefault(c => c.Id == categoria.Id);
                if (categoriaExistente == null)
                    return NotFound();

                var totalActual = _context.Categorias
                    .Where(c => c.Activa && c.Id != categoria.Id)
                    .Sum(c => (decimal?)c.PorcentajeMaximo) ?? 0;

                var nuevoTotal = totalActual + categoria.PorcentajeMaximo;

                if (categoria.PorcentajeMaximo > 0 && nuevoTotal > 100)
                {
                    ModelState.AddModelError("PorcentajeMaximo", $"No puedes asignar este porcentaje porque el total superaría el 100% (actual sin esta: {totalActual}%).");
                    return View(categoria);
                }

                categoriaExistente.Titulo = categoria.Titulo;
                categoriaExistente.Descripcion = categoria.Descripcion;
                categoriaExistente.PorcentajeMaximo = categoria.PorcentajeMaximo;
                categoriaExistente.Activa = categoria.Activa;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // ELIMINAR (GET)
        public IActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}



