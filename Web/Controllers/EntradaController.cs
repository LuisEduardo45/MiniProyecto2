using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace MvcTemplate.Controllers
{
    public class EntradaController : Controller
    {
        public static List<Entrada> _entradas = new List<Entrada>();
        public static List<Categoria> _categorias => GastoController._categorias;

        public IActionResult Index()
        {
            var entradas = _entradas.OrderByDescending(e => e.Fecha).ToList();
            return View(entradas);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_categorias.Where(c => c.Activa).ToList(), "Id", "Titulo");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Entrada entrada)
        {
            ViewBag.Categorias = new SelectList(_categorias.Where(c => c.Activa).ToList(), "Id", "Titulo");

            // DEBUG: Verifica el valor que recibe el modelo
            Debug.WriteLine($"CategoriaId recibido: {entrada.CategoriaId}");

            if (ModelState.IsValid)
            {
                entrada.Id = _entradas.Count > 0 ? _entradas.Max(e => e.Id) + 1 : 1;
                entrada.Categoria = _categorias.FirstOrDefault(c => c.Id == entrada.CategoriaId);
                _entradas.Add(entrada);
                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // ... resto igual ...
    }
}