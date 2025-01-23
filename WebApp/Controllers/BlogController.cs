using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    [Route("zone/blog")]
    public class BlogController : Controller
    {

        public BlogController()
        {
            
        }
        public IActionResult Index()
        {
            return View(nameof(Index));
        }
    }
}
