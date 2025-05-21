using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Data;
using MvcTemplate.Models;
using System.Linq;

namespace MvcTemplate.Controllers
{
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GastoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Gasto
        public IActionResult Index()
        {
            var gastos = _context.Gastos.ToList();
            return View(gastos); // Busca Views/Gasto/Index.cshtml
        }

        // GET: Gasto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gasto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Gastos.Add(gasto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(gasto);
        }

        // GET: Gasto/Edit/5
        public IActionResult Edit(int id)
        {
            var gasto = _context.Gastos.Find(id);
            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // POST: Gasto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Gastos.Update(gasto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(gasto);
        }

        // GET: Gasto/Delete/5
        public IActionResult Delete(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // POST: Gasto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gasto = _context.Gastos.Find(id);
            if (gasto != null)
            {
                _context.Gastos.Remove(gasto);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

