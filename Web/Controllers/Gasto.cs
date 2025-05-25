using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System;
using System.Linq;
using System.Security.Claims;

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Mostrar solo los gastos del usuario autenticado
            var gastos = _context.Gastos
                .Include(g => g.Categoria)
                .Where(g => g.UsuarioId == userId)
                .ToList();

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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                gasto.UsuarioId = userId;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id && g.UsuarioId == userId);
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var gastoExistente = _context.Gastos.FirstOrDefault(g => g.Id == gasto.Id && g.UsuarioId == userId);
                if (gastoExistente == null)
                    return NotFound();

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var gasto = _context.Gastos
                .Include(g => g.Categoria)
                .FirstOrDefault(g => g.Id == id && g.UsuarioId == userId);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id && g.UsuarioId == userId);
            if (gasto == null)
                return NotFound();

            _context.Gastos.Remove(gasto);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
