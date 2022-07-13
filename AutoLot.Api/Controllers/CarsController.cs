using System.Collections.Generic;
using AutoLot.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using AutoLot.Model.Entities;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;


namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")]
    public class CarsController : BaseCrudController<Car, CarsController>
    {
        public CarsController(ICarRepository carRepository, IAppLogging<CarsController> logger) : base(carRepository, logger) { }


        /// <summary>
        /// Get all cars by Make
        /// </summary>
        /// <param name="id">PK of Make</param>
        /// <returns>All cars for specified with id make.id</returns>
        /// <response code="200">Returns all cars by make</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(204, "No content")]
        [HttpGet("bymake/{id?}")]
        public ActionResult<IEnumerable<Car>> GetCarsByMake(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return Ok(((ICarRepository)MainRepository).FindAllBy(id.Value));
            }

            return Ok(MainRepository.FindAllIgnoreQueryFilters());
        }
    }
}
