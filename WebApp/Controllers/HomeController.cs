using AppCore.Services.Helpers;
using AppGlobal.Services.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using OnaxTools.Services.StackExchangeRedis.Interface;
using Polly;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICacheService cacheService;
        private readonly IAppLogger<HomeController> _applogger;
        private readonly IPollyService<GenResponse<string>, Exception> _pollyService;

        public HomeController(ILogger<HomeController> logger, ICacheService cacheService
            , IAppLogger<HomeController> applogger, IPollyService<GenResponse<string>, Exception> pollyService)
        {
            _logger = logger;
            this.cacheService = cacheService;
            _applogger = applogger;
            _pollyService = pollyService;
        }
        public async Task<IActionResult> Index()
        {
            _ = await cacheService.SetData<List<string>>("categories", new List<string> { "Category 1", "Category 2", "Category 3" }, 60);
            var returnedValue = await cacheService.GetData<List<string>>("categories");
            return View(nameof(Index));
        }

        public async Task<IActionResult> Privacy()
        {
            GenResponse<string> resp = new();
            await _pollyService.ResiliencePipeline(fallbackAction: _ => Outcome.FromResultAsValueTask(GenResponse<string>.Failed("Failed to save to DB")))
                .ExecuteAsync(async (cancellationToken) =>
            {
                resp = await _applogger.LogInformationAsync("Resilience with Polly and DI implementation.");
                return resp;
            });
            return View(nameof(Privacy));
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(nameof(Login));
        }
    }
}
