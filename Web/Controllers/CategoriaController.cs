using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTADO
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var categorias = _context.Categorias.Where(c => c.UsuarioId == userId).ToList();
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
        public async Task<IActionResult> Create(Categoria categoria)
        {
            // Asignar UsuarioId antes de validar
            categoria.UsuarioId = _userManager.GetUserId(User);
            ModelState.Remove(nameof(categoria.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            // Aquí puedes seguir con la lógica que ya tienes (validar porcentaje, etc.)

            var totalActual = _context.Categorias
                .Where(c => c.Activa && c.UsuarioId == categoria.UsuarioId)
                .Sum(c => (decimal?)c.PorcentajeMaximo) ?? 0;

            var nuevoTotal = totalActual + categoria.PorcentajeMaximo;

            if (categoria.PorcentajeMaximo > 0 && nuevoTotal > 100)
            {
                ModelState.AddModelError("PorcentajeMaximo", $"No puedes agregar esta categoría porque el total superaría el 100% (actual: {totalActual}%).");
                return View(categoria);
            }

            categoria.Activa = true;
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id && c.UsuarioId == userId);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var categoriaExistente = _context.Categorias.FirstOrDefault(c => c.Id == categoria.Id && c.UsuarioId == userId);
                if (categoriaExistente == null)
                    return NotFound();

                var totalActual = _context.Categorias
                    .Where(c => c.Activa && c.Id != categoria.Id && c.UsuarioId == userId)
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
            var userId = _userManager.GetUserId(User);
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id && c.UsuarioId == userId);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id && c.UsuarioId == userId);
            if (categoria == null)
                return NotFound();

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
