using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GastoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var gastos = await _context.Gastos
                .Where(g => g.UsuarioId == userId)
                .Include(g => g.Categoria)
                .ToListAsync();

            return View(gastos);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Categorias = _context.Categorias
                .Where(c => c.Activa && c.UsuarioId == userId)
                .ToList();
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gasto gasto)
        {
            var userId = _userManager.GetUserId(User);
            gasto.UsuarioId = userId;
            ModelState.Remove(nameof(gasto.UsuarioId));

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == gasto.CategoriaId && c.Activa && c.UsuarioId == userId);

            if (categoria == null)
            {
                ModelState.AddModelError("", "Categoría no válida.");
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            decimal totalEntradas = await _context.Entradas
                .Where(e => e.UsuarioId == userId)
                .SumAsync(e => e.Valor);

            decimal gastoActual = await _context.Gastos
                .Where(g => g.CategoriaId == gasto.CategoriaId && g.UsuarioId == userId)
                .SumAsync(g => g.Monto);

            decimal maximoPermitido = (categoria.PorcentajeMaximo / 100m) * totalEntradas;

            if (gastoActual + gasto.Monto > maximoPermitido)
            {
                ModelState.AddModelError("", $"El gasto excede el tope máximo permitido de {maximoPermitido:C} para la categoría '{categoria.Titulo}'.");
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            try
            {
                _context.Gastos.Add(gasto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error guardando el gasto en la base de datos: " + ex.Message);
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDITAR (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == userId);

            if (gasto == null)
                return NotFound();

            ViewBag.Categorias = _context.Categorias
                .Where(c => c.Activa && c.UsuarioId == userId)
                .ToList();

            return View(gasto);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Gasto gasto)
        {
            var userId = _userManager.GetUserId(User);
            gasto.UsuarioId = userId;
            ModelState.Remove(nameof(gasto.UsuarioId));

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            var gastoExistente = await _context.Gastos
                .FirstOrDefaultAsync(g => g.Id == gasto.Id && g.UsuarioId == userId);

            if (gastoExistente == null)
                return NotFound();

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == gasto.CategoriaId && c.Activa && c.UsuarioId == userId);

            if (categoria == null)
            {
                ModelState.AddModelError("", "Categoría no válida.");
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            decimal totalEntradas = await _context.Entradas
                .Where(e => e.UsuarioId == userId)
                .SumAsync(e => e.Valor);

            decimal gastoActual = await _context.Gastos
                .Where(g => g.CategoriaId == gasto.CategoriaId && g.Id != gasto.Id && g.UsuarioId == userId)
                .SumAsync(g => g.Monto);

            decimal maximoPermitido = (categoria.PorcentajeMaximo / 100m) * totalEntradas;

            if (gastoActual + gasto.Monto > maximoPermitido)
            {
                ModelState.AddModelError("", $"El gasto excede el tope máximo permitido de {maximoPermitido:C} para la categoría '{categoria.Titulo}'.");
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            gastoExistente.Descripcion = gasto.Descripcion;
            gastoExistente.Monto = gasto.Monto;
            gastoExistente.Fecha = gasto.Fecha;
            gastoExistente.CategoriaId = gasto.CategoriaId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error actualizando el gasto en la base de datos: " + ex.Message);
                ViewBag.Categorias = _context.Categorias
                    .Where(c => c.Activa && c.UsuarioId == userId)
                    .ToList();
                return View(gasto);
            }

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var gasto = await _context.Gastos
                .Include(g => g.Categoria)
                .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == userId);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var gasto = await _context.Gastos.FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == userId);

            if (gasto == null)
                return NotFound();

            _context.Gastos.Remove(gasto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error eliminando el gasto en la base de datos: " + ex.Message);
                return View(gasto);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

