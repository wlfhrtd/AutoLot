using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AutoLot.Api.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostEnvironment;


        public CustomExceptionFilterAttribute(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }


        public override void OnException(ExceptionContext context)
        {
            var e = context.Exception;
            string stackTrace = _hostEnvironment.IsDevelopment() ? context.Exception.StackTrace : string.Empty;
            string message = e.Message;

            string error = null;
            IActionResult actionResult = null;

            switch (e)
            {
                case DbUpdateConcurrencyException ce:
                    error = "Concurrency issue occurred";
                    // 400
                    actionResult = new BadRequestObjectResult(
                        new { Error = error, Message = message, StackTrace = stackTrace });

                    break;
                default:
                    error = "General error";
                    actionResult = new ObjectResult(
                        new { Error = error, Message = message, StackTrace = stackTrace }) { StatusCode = 500 };

                    break;
            }

            /*
             * this allows to send 200 response and swallow exception for handle&log without noticing to client 
             * 
               context.ExceptionHandled = true;
            */

            context.Result = actionResult;
        }
    }
}
