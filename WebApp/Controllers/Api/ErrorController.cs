using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers.Filters;

namespace WebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("exception")]
        [AuthAttributeDynamicOnx]
        public IActionResult CreateAppException()
        {
            throw new Exception("This is a test exception");
        }
    }
}
