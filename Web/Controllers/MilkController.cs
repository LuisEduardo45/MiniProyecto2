using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.IServices;

namespace MvcTemplate.Controllers
{
    [Authorize]
    public class MilkController : Controller
    {
        private IService Service { get; set; }
        public MilkController(IService service)
        {
            Service = service;
        }
        public async Task<IActionResult> Index()
        {

            MilkModels model = new MilkModels();
            model.Milks = await Service.GetAllMilks();
            return View(model);
        }


        public IActionResult AddMilk()
        {

            MilkModel model = new MilkModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMilk(MilkModel model)
        {
            if (ModelState.IsValid)
            {
                await Service.AddMilk(model.Liters, model.RecolectionDate);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
