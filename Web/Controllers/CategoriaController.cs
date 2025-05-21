using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class CategoriaController : Controller
    {
        private static List<Categoria> _categorias = new List<Categoria>();

        // LISTADO
        public IActionResult Index()
        {
            return View(_categorias);
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
                categoria.Id = _categorias.Count > 0 ? _categorias.Max(c => c.Id) + 1 : 1;
                _categorias.Add(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            var categoriaExistente = _categorias.FirstOrDefault(c => c.Id == categoria.Id);
            if (categoriaExistente == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                categoriaExistente.Titulo = categoria.Titulo;
                categoriaExistente.Descripcion = categoria.Descripcion;
                categoriaExistente.PorcentajeMaximo = categoria.PorcentajeMaximo;

                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // ELIMINAR (GET) - Confirmación
        public IActionResult Delete(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // ELIMINAR (POST) - Acción real
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                return NotFound();

            _categorias.Remove(categoria);
            return RedirectToAction(nameof(Index));
        }
    }
}
