using AppGlobal.Models.Speech;
using AppGlobal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.Net;

namespace WebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly ISpeechService _speechSvc;
        private readonly IAppSessionContextRepository _sessionRepo;

        public SpeechController(ISpeechService speechSvc, IAppSessionContextRepository _sessionRepo)
        {
            _speechSvc = speechSvc;
            this._sessionRepo = _sessionRepo;
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetVoices))]
        [ProducesResponseType(typeof(GenResponse<Array>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetVoices()
        {
            return Ok(await Task.Run(() => _speechSvc.GetAllVoices()));
        }

        [AllowAnonymous]
        [HttpPost(nameof(ConvertToSpeech))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConvertToSpeech(TextToSpeechDto model)
        {
            AppSessionData<AppUserIdentity> user = _sessionRepo.GetUserDataFromSession();
            var objResp = await _speechSvc.ConvertTextToSpeach(model, user.Data);
            return Ok(objResp);
        }
    }
}
