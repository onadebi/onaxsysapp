using AppGlobal.Services.Logger;
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
        private readonly IAppLogger<HomeController> _applogger;

        public HomeController(ILogger<HomeController> logger, ICacheService cacheService,IAppLogger<HomeController> applogger)
        {
            _logger = logger;
            this.cacheService = cacheService;
            _applogger = applogger;
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
        public async Task<IActionResult> Login()
        {
            var resp = await _applogger.LogInformationAsync("User visited login page");
            return View(nameof(Login));
        }
    }
}
