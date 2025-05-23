using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class EntradaController : Controller
    {
        // Lista estática de entradas (compartida)
        public static List<Entrada> _entradas = new List<Entrada>();

        // GET: Entrada
        public IActionResult Index()
        {
            var entradas = _entradas.OrderByDescending(e => e.Fecha).ToList();
            return View(entradas);
        }

        // GET: Entrada/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrada/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Entrada entrada)
        {
            if (ModelState.IsValid)
            {
                entrada.Id = _entradas.Count > 0 ? _entradas.Max(e => e.Id) + 1 : 1;
                _entradas.Add(entrada);
                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // GET: Entrada/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var entrada = _entradas.FirstOrDefault(e => e.Id == id);
            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // POST: Entrada/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Entrada entrada)
        {
            if (id != entrada.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existente = _entradas.FirstOrDefault(e => e.Id == id);
                if (existente == null) return NotFound();

                existente.Descripcion = entrada.Descripcion;
                existente.Valor = entrada.Valor;
                existente.Fecha = entrada.Fecha;

                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // GET: Entrada/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var entrada = _entradas.FirstOrDefault(e => e.Id == id);
            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // POST: Entrada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var entrada = _entradas.FirstOrDefault(e => e.Id == id);
            if (entrada != null)
            {
                _entradas.Remove(entrada);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}