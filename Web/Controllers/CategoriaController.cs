using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;

namespace MvcTemplate.Controllers
{
    public class CategoriaController : Controller
    {
        private static List<Categoria> _categorias = new List<Categoria>(); // temporal (luego usar DB)

        public IActionResult Index()
        {
            return View(_categorias);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.Id = _categorias.Count + 1;
                _categorias.Add(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }
    }
}
