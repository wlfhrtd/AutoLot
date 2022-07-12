using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")] // matches __domain__/api/values
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Example Get() returning json
        /// </summary>
        /// <remarks>Example of returned json:
        /// <pre>
        /// [
        ///   "value1",
        ///   "value2"
        /// ]
        /// </pre>
        /// </remarks>
        /// <returns>Array of strings</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Fail")]
        public IActionResult Get()
        {
            return Ok(new string[] { "value1", "value2" });
        }

        [HttpGet("one")]
        public IEnumerable<string> Get1()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("two")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Fail")]
        public ActionResult<IEnumerable<string>> Get2()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("three")]
        public string[] Get3()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("four")]
        public IActionResult Get4()
        {
            return new JsonResult(new string[] { "value1", "value2" });
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return NotFound();
        }
    }
}
