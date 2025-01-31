using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApp.Controllers
{
    //[AllowAnonymous]
    [Route("zone/blog")]
    public class BlogController : Controller
    {
        public BlogController()
        {

        }

        [HttpGet]
        [SwaggerIgnore]
        public IActionResult Index()
        {
            return View(nameof(Index));
        }
    }
}
