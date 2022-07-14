using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Model.Entities;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AutoLot.Mvc.Controllers
{
    [Route("[controller]/[action]")]
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IAppLogging<CarsController> _logger;


        public CarsController(ICarRepository carRepository, IAppLogging<CarsController> logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }


        [Route("/[controller]")]
        [Route("/[controller]/[action]")]
        public IActionResult Index()
        {
            return View(_carRepository.FindAllIgnoreQueryFilters());
        }

        [HttpGet]
        public IActionResult Create([FromServices] IMakeRepository makeRepository)
        {
            ViewData["MakeId"] = GetMakes(makeRepository);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromServices] IMakeRepository makeRepository, Car car)
        {
            if (ModelState.IsValid)
            {
                _carRepository.Add(car);

                return RedirectToAction(nameof(Details), new { id = car.Id });
            }
            
            ViewData["MakeId"] = GetMakes(makeRepository);

            return View();
        }

        [HttpGet("{id?}")]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            Car car = GetOne(id);

            return car == null ? NotFound() : View(car);
        }

        [HttpGet("{id?}")]
        public IActionResult Delete(int? id)
        {
            Car car = GetOne(id);

            return car == null ? NotFound() : View(car);
        }

        [HttpPost("{id}")]
        public IActionResult Delete(int id, Car car)
        {
            if (id!=car.Id)
            {
                return BadRequest();
            }

            _carRepository.Remove(car);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id?}")]
        public IActionResult Edit([FromServices] IMakeRepository makeRepository, int? id)
        {
            Car car = GetOne(id);
            if (car == null)
            {
                return NoContent();
            }

            ViewData["MakeId"] = GetMakes(makeRepository);

            return View(car);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromServices] IMakeRepository makeRepository, int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _carRepository.Update(car);
                return RedirectToAction(nameof(Details), new { id = car.Id });
            }

            ViewData["MakeId"] = GetMakes(makeRepository);

            return View(car);
        }

        [HttpGet("/[controller]/[action]/{makeId}/{makeName}")]
        public IActionResult ByMake(int makeId, string makeName)
        {
            ViewBag.MakeName = makeName;

            return View(_carRepository.FindAllBy(makeId));
        }

        internal Car GetOne(int? id)
        {
            return !id.HasValue ? null : _carRepository.FindOneById(id.Value);
        }

        internal SelectList GetMakes(IMakeRepository makeRepository)
        {
            return new(makeRepository.FindAll(), nameof(Make.Id), nameof(Make.Name));
        }
    }
}
