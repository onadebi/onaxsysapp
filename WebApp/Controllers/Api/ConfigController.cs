using AppGlobal.Models.Gemini;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using static WebApp.Controllers.Api.AIController;
using System.Net;
using AppGlobal.Services;
using WebApp.Helpers.Filters;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ConfigController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public ConfigController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [AuthAttributeDynamicOnx]
        [HttpPost(nameof(gemini))]
        [ProducesResponseType(typeof(GenResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> gemini([FromBody] SearchParams search)
        {
            GeminiRequestBody geminiRequestBody = new GeminiRequestBody().Add([new() { role = "user", parts = [new() { text = search.query }] }]);
            GenResponse<GeminiResponseBody?> objResp = await _geminiService.GeminiQueryRequest(geminiRequestBody);
            return StatusCode(objResp.StatCode, objResp);
        }
    }
}
