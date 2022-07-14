using System.Threading.Tasks;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Model.Entities;
using AutoLot.Services.ApiWrapper;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AutoLot.Mvc.Controllers
{
    [Route("[controller]/[action]")]
    public class CarsController : Controller
    {
        private readonly IApiServiceWrapper _serviceWrapper;
        private readonly IAppLogging<CarsController> _logger;


        public CarsController(IApiServiceWrapper serviceWrapper, IAppLogging<CarsController> logger)
        {
            _serviceWrapper = serviceWrapper;
            _logger = logger;
        }


        [Route("/[controller]")]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Index()
            => View(await _serviceWrapper.GetCarsAsync());

        [HttpGet("{makeId}/{makeName}")]
        public async Task<IActionResult> ByMake(int makeId, string makeName)
        {
            ViewBag.MakeName = makeName;

            return View(await _serviceWrapper.GetCarsByMakeAsync(makeId));
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var car = await GetOneAsync(id);

            return car == null ? NotFound() : View(car);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["MakeId"] = await GetMakesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                await _serviceWrapper.AddCarAsync(car);
                return RedirectToAction(nameof(Index));
            }

            ViewData["MakeId"] = await GetMakesAsync();
            return View(car);
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            var car = await GetOneAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            ViewData["MakeId"] = await GetMakesAsync();

            return View(car);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _serviceWrapper.UpdateCarAsync(id, car);
                return RedirectToAction(nameof(Index));
            }

            ViewData["MakeId"] = await GetMakesAsync();
            return View(car);
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var car = await GetOneAsync(id);
            return car == null ? NotFound() : View(car);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Car car)
        {
            await _serviceWrapper.DeleteCarAsync(id, car);
            return RedirectToAction(nameof(Index));
        }

        internal async Task<Car> GetOneAsync(int? id)
            => !id.HasValue ? null : await _serviceWrapper.GetCarAsync(id.Value);

        internal async Task<SelectList> GetMakesAsync()
            => new SelectList(
                await _serviceWrapper.GetMakesAsync(),
                nameof(Make.Id),
                nameof(Make.Name)
                );
    }
}
