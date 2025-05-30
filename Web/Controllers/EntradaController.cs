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
    public class EntradaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EntradaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entradas = await _context.Entradas
                .Where(e => e.UsuarioId == userId)
                .ToListAsync();

            return View(entradas);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Entrada entrada)
        {
            entrada.UsuarioId = _userManager.GetUserId(User);
            ModelState.Remove(nameof(entrada.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(entrada);
            }

            try
            {
                _context.Entradas.Add(entrada);
                var rows = await _context.SaveChangesAsync();

                if (rows == 0)
                {
                    ModelState.AddModelError("", "No se guardó ninguna entrada en la base de datos.");
                    return View(entrada);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error guardando la entrada en la base de datos: " + ex.Message);
                return View(entrada);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDITAR (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entrada = await _context.Entradas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);

            if (entrada == null)
                return NotFound();

            return View(entrada);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Entrada entrada)
        {
            var userId = _userManager.GetUserId(User);
            entrada.UsuarioId = userId;
            ModelState.Remove(nameof(entrada.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(entrada);
            }

            var entradaExistente = await _context.Entradas
                .FirstOrDefaultAsync(e => e.Id == entrada.Id && e.UsuarioId == userId);

            if (entradaExistente == null)
                return NotFound();

            entradaExistente.Descripcion = entrada.Descripcion;
            entradaExistente.Valor = entrada.Valor;
            entradaExistente.Fecha = entrada.Fecha;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error actualizando la entrada en la base de datos: " + ex.Message);
                return View(entrada);
            }

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entrada = await _context.Entradas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);

            if (entrada == null)
                return NotFound();

            return View(entrada);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entrada = await _context.Entradas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);

            if (entrada == null)
                return NotFound();

            _context.Entradas.Remove(entrada);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error eliminando la entrada en la base de datos: " + ex.Message);
                return View(entrada);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
