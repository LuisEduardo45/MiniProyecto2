using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class GastoController : Controller
    {
        private static List<Gasto> _gastos = new List<Gasto>();

        // LISTAR
        public IActionResult Index()
        {
            return View(_gastos);
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                gasto.Id = _gastos.Count > 0 ? _gastos.Max(g => g.Id) + 1 : 1;
                _gastos.Add(gasto);
                return RedirectToAction(nameof(Index));
            }

            return View(gasto);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Gasto gasto)
        {
            var existente = _gastos.FirstOrDefault(g => g.Id == gasto.Id);
            if (existente == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existente.Descripcion = gasto.Descripcion;
                existente.Monto = gasto.Monto;
                existente.Fecha = gasto.Fecha;
                return RedirectToAction(nameof(Index));
            }

            return View(gasto);
        }

        // ELIMINAR (GET) - Confirmación
        public IActionResult Delete(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // ELIMINAR (POST) - Acción real
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gasto = _gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            _gastos.Remove(gasto);
            return RedirectToAction(nameof(Index));
        }
    }
}


