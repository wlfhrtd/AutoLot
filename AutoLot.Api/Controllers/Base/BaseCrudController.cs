using System;
using System.Collections.Generic;
using AutoLot.Dal.Exceptions;
using AutoLot.Model.Entities.Base;
using AutoLot.Dal.Repository.Base;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace AutoLot.Api.Controllers.Base
{
    [ApiController] // auto ModelState isValid check and 400 return if false
    public abstract class BaseCrudController<T, TController> : ControllerBase
        where T : BaseEntity, new()
        where TController : BaseCrudController<T, TController>
    {
        protected readonly IRepository<T> MainRepository;
        protected readonly IAppLogging<TController> Logger;


        protected BaseCrudController(IRepository<T> repository, IAppLogging<TController> logger)
        {
            MainRepository = repository;
            Logger = logger;
        }


        /// <summary>
        /// Find all
        /// </summary>
        /// <returns>All records</returns>
        /// <response code="200">Returns all items</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Failed")]
        [HttpGet]
        public ActionResult<IEnumerable<T>> GetAll()
        {
            return Ok(MainRepository.FindAllIgnoreQueryFilters());
        }

        /// <summary>
        /// Find one by id
        /// </summary>
        /// <param name="id">Primary key of record</param>
        /// <returns>Single record</returns>
        /// <response code="200">Found record</response>
        /// <response code="204">No content</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(204, "No content")]
        [HttpGet("{id}")] // FromRoute model binding
        public ActionResult<T> GetOne(int id)
        {
            var entity = MainRepository.FindOneById(id);
            return entity == null ? NotFound() : Ok(entity);
        }

        /// <summary>
        /// Update one
        /// </summary>
        /// <remarks>
        /// Sample body:
        /// <pre>
        /// {
        ///   "Id": 1,
        ///   "TimeStamp": "AAAAAAAAB+E=",
        ///   "MakeId": 1,
        ///   "Color": "Black",
        ///   "PetName": "Zippy",
        ///   "MakeColor": "VW (Black)",
        /// }  
        /// </pre>
        /// </remarks>
        /// <param name="id">Primary key of record to update</param>
        /// <param name="entity">Entity to update</param>
        /// <returns>Single record</returns>
        /// <response code="200">Found and updated record</response>
        /// <response code="400">Bad request</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Invalid request")]
        [HttpPut("{id}")] // FromRoute model binding
        public IActionResult UpdateOne(int id, T entity) // entity is FromBody
        {
            if (id != entity.Id) return BadRequest();

            try
            {
                MainRepository.Update(entity);
            }
            catch (CustomException e)
            {
                Logger.LogAppError(e, e.Message + "; " + e.InnerException?.Message);
                return BadRequest(e);
            }
            catch (Exception e)
            {
                Logger.LogAppError(e, e.Message + "; " + e.InnerException?.Message);
                return BadRequest(e);
            }

            return Ok(entity);
        }

        /// <summary>
        /// Adds one record
        /// </summary>
        /// <remarks>
        /// Sample body:
        /// <pre>
        /// {
        ///   "Id": 1,
        ///   "TimeStamp": "AAAAAAAAB+E=",
        ///   "MakeId": 1,
        ///   "Color": "Black",
        ///   "PetName": "Zippy",
        ///   "MakeColor": "VW (Black)",
        /// }  
        /// </pre>
        /// </remarks>
        /// <param name="entity">Entity to persist</param>
        /// <returns>Added record</returns>
        /// <response code="201">Added record</response>
        /// <response code="400">Bad request</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(201, "Success")]
        [SwaggerResponse(400, "Invalid request")]
        [HttpPost]
        public ActionResult<T> AddOne(T entity)
        {
            try
            {
                MainRepository.Add(entity);
            }
            catch (Exception e)
            {
                Logger.LogAppError(e, e.Message + "; " + e.InnerException?.Message);
                return BadRequest(e);
            }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, entity); // returns url to added entity with Location header, json body is added entity
        }

        /// <summary>
        /// Delete single record
        /// </summary>
        /// <remarks>
        /// Sample body:
        /// <pre>
        /// {
        ///   "Id": 1,
        ///   "TimeStamp": "AAAAAAAAB+E=",
        /// }
        /// </pre>
        /// </remarks>
        /// <param name="id">Primary key</param>
        /// <param name="entity">Entity to delete</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Found and deleted record</response>
        /// <response code="400">Bad request</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Invalid request")]
        [HttpDelete("{id}")]
        public ActionResult<T> DeleteOne(int id, T entity)
        {
            if (id != entity.Id) return BadRequest();

            try
            {
                MainRepository.Remove(entity);
            }
            catch (Exception e)
            {
                Logger.LogAppError(e, e.Message + "; " + e.InnerException?.Message);
                return new BadRequestObjectResult(e.GetBaseException()?.Message);
            }

            return Ok();
        }
    }
}
