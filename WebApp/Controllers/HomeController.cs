using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View(nameof(Privacy));
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(nameof(Login));
        }
    }
}
