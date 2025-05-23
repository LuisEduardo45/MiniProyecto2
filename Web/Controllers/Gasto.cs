using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class GastoController : Controller
    {


        // Lista estática de categorías, igual a la que usas en CategoriaController
        private static List<Categoria> _categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Titulo = "Alimentación", TopeMaximo = 0, PorcentajeMaximo = 30, Activa = true },
            new Categoria { Id = 2, Titulo = "Transporte", TopeMaximo = 0, PorcentajeMaximo = 20, Activa = true },
            // Más categorías si quieres
        };

        // Lista estática de gastos
        private static List<Gasto> _gastos = new List<Gasto>();

        // Simula entradas de dinero para calcular topes
        private static decimal TotalEntradas = 1000000; // Ejemplo: un millón

        // LISTAR gastos con nombre de categoría
        public IActionResult Index()
        {
            // Incluir categoría en cada gasto para la vista
            foreach (var gasto in _gastos)
            {
                gasto.Categoria = _categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId);
            }
            return View(_gastos);
        }

        // CREAR (GET) - enviamos lista de categorías para dropdown
        public IActionResult Create()
        {
            ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                var categoria = _categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId);
                if (categoria == null)
                {
                    ModelState.AddModelError("CategoriaId", "La categoría no existe.");
                    ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                // Calcular TopeMaximo de la categoría basado en el porcentaje
                categoria.TopeMaximo = TotalEntradas * categoria.PorcentajeMaximo / 100;

                // Calcular gasto total actual de esa categoría en el mes y sumar el nuevo gasto
                var gastoMesCategoria = _gastos
                    .Where(g => g.CategoriaId == categoria.Id && g.Fecha.Month == gasto.Fecha.Month && g.Fecha.Year == gasto.Fecha.Year)
                    .Sum(g => g.Monto);
                var nuevoTotal = gastoMesCategoria + gasto.Monto;

                if (nuevoTotal > categoria.TopeMaximo)
                {
                    ModelState.AddModelError("", $"El gasto supera el tope máximo de la categoría ({categoria.TopeMaximo:C}).");
                    ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                gasto.Id = _gastos.Count > 0 ? _gastos.Max(g => g.Id) + 1 : 1;
                gasto.Categoria = categoria;
                _gastos.Add(gasto);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null) return NotFound();

            ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Gasto gasto)
        {
            var existente = _gastos.FirstOrDefault(g => g.Id == gasto.Id);
            if (existente == null) return NotFound();

            if (ModelState.IsValid)
            {
                var categoria = _categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId);
                if (categoria == null)
                {
                    ModelState.AddModelError("CategoriaId", "La categoría no existe.");
                    ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                categoria.TopeMaximo = TotalEntradas * categoria.PorcentajeMaximo / 100;

                var gastoMesCategoria = _gastos
                    .Where(g => g.CategoriaId == categoria.Id && g.Fecha.Month == gasto.Fecha.Month && g.Fecha.Year == gasto.Fecha.Year && g.Id != gasto.Id)
                    .Sum(g => g.Monto);

                var nuevoTotal = gastoMesCategoria + gasto.Monto;

                if (nuevoTotal > categoria.TopeMaximo)
                {
                    ModelState.AddModelError("", $"El gasto supera el tope máximo de la categoría ({categoria.TopeMaximo:C}).");
                    ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
                    return View(gasto);
                }

                existente.Descripcion = gasto.Descripcion;
                existente.Monto = gasto.Monto;
                existente.Fecha = gasto.Fecha;
                existente.CategoriaId = gasto.CategoriaId;
                existente.Categoria = categoria;

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _categorias.Where(c => c.Activa).ToList();
            return View(gasto);
        }

        // ELIMINAR (GET)
        public IActionResult Delete(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null) return NotFound();

            gasto.Categoria = _categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId);
            return View(gasto);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null) return NotFound();

            _gastos.Remove(gasto);
            return RedirectToAction(nameof(Index));
        }
    }
}
