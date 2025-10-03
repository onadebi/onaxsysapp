using AppCore.Domain.AppCore.Models;
using AppCore.GenericRepo;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeminiService _geminiService;

    public AIController(IGeminiService geminiService, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _geminiService = geminiService;
    }

    [HttpPost(nameof(gemini))]
    [ProducesResponseType(typeof(GenResponse<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> gemini([FromBody] SearchParams search)
    {
        GeminiRequestBody geminiRequestBody = new GeminiRequestBody().Add([new() { role = "user", parts = [new() { text = search.query }] }]);
        GenResponse<GeminiResponseBody?> objResp = await _geminiService.GeminiQueryRequest(geminiRequestBody);
        IEnumerable<UserProfile> allUsers = await _unitOfWork.Repository<UserProfile>().GetAllPagedAsync();
        return StatusCode(objResp.StatCode, objResp);
    }

    #region HELPERS
    public record SearchParams(string query);
    #endregion
}
