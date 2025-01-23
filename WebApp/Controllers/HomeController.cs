using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Services.StackExchangeRedis.Interface;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICacheService cacheService;

        public HomeController(ILogger<HomeController> logger, ICacheService cacheService)
        {
            _logger = logger;
            this.cacheService = cacheService;
        }
        public async Task<IActionResult> Index()
        {
            _ = await cacheService.SetData<List<string>>("categories", new List<string> { "Category 1", "Category 2", "Category 3" }, 60);
            var returnedValue = await cacheService.GetData<List<string>>("categories");
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
