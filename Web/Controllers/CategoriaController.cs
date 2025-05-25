using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using MvcTemplate.Data; // Contexto de BD
using System.Linq;

namespace MvcTemplate.Controllers
{
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
                // Valores por defecto
                categoria.Activa = true;
               // categoria.TopeMaximo = 0;

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

                categoriaExistente.Titulo = categoria.Titulo;
                categoriaExistente.Descripcion = categoria.Descripcion;
                categoriaExistente.PorcentajeMaximo = categoria.PorcentajeMaximo;

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


