using AppGlobal.Models.Gemini;
using AppGlobal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using System.Net;

namespace WebApp.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class AIController : ControllerBase
{
    private readonly IGeminiService _geminiService;

    public AIController(IGeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [AllowAnonymous]
    [HttpPost(nameof(gemini))]
    [ProducesResponseType(typeof(GenResponse<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> gemini([FromBody] SearchParams search)
    {
        GeminiRequestBody geminiRequestBody = new GeminiRequestBody().Add([new() { role = "user", parts = [new() { text = search.query }] }]);
        GenResponse<GeminiResponseBody?> objResp = await _geminiService.GeminiQueryRequest(geminiRequestBody);
        return StatusCode(objResp.StatCode, objResp);
    }

    #region HELPERS
    public record SearchParams(string query);
    #endregion
}
