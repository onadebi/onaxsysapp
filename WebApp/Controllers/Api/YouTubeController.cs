using AppGlobal.Models.Gemini;
using AppGlobal.Models.YouTube;
using AppGlobal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using System.Net;

namespace WebApp.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class YouTubeController : ControllerBase
{
    private readonly IYouTubeService _youTubeService;

    public YouTubeController(IYouTubeService youTubeService)
    {
        _youTubeService = youTubeService;
    }

    [AllowAnonymous]
    [HttpPost(nameof(Search))]
    [ProducesResponseType(typeof(GenResponse<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromBody]YouTubeQueryRequest search)
    {
        var objResp = await _youTubeService.YouTubeApiQueryRequest(search);
        return StatusCode(objResp.StatCode, objResp);
    }

    #region HELPERS
    #endregion
}
