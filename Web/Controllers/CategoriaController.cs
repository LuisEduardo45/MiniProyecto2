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
            categoria.UsuarioId = _userManager.GetUserId(User);
            ModelState.Remove(nameof(categoria.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

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

            try
            {
                _context.Categorias.Add(categoria);
                var rows = await _context.SaveChangesAsync();

                if (rows == 0)
                {
                    ModelState.AddModelError("", "No se guardó ninguna categoría en la base de datos.");
                    return View(categoria);
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error guardando la categoría: " + ex.Message);
                return View(categoria);
            }

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
        public async Task<IActionResult> Edit(Categoria categoria)
        {
            var userId = _userManager.GetUserId(User);
            ModelState.Remove(nameof(categoria.UsuarioId));

            if (!ModelState.IsValid)
                return View(categoria);

            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == categoria.Id && c.UsuarioId == userId);

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

            try
            {
                var rows = await _context.SaveChangesAsync();
                if (rows == 0)
                {
                    ModelState.AddModelError("", "No se actualizó ninguna categoría.");
                    return View(categoria);
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error actualizando la categoría: " + ex.Message);
                return View(categoria);
            }

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (categoria == null)
                return NotFound();

            try
            {
                _context.Categorias.Remove(categoria);
                var rows = await _context.SaveChangesAsync();

                if (rows == 0)
                {
                    ModelState.AddModelError("", "No se eliminó ninguna categoría.");
                    return View(categoria);
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error eliminando la categoría: " + ex.Message);
                return View(categoria);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
