using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DakarRally.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public ActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            var message = exception.Message;
            switch (exception)
            {
                case ArgumentNullException:
                    return NotFound(message);
                case NullReferenceException:
                    return NotFound(message);
                case Exception:
                    return BadRequest(message);
                default:
                    return BadRequest(message);
            }
        }
    }
}
