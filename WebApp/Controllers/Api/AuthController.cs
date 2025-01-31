using AppGlobal.Config;
using AppGlobal.Models;
using AppGlobal.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.Net;
using System.Security.Claims;
using TokenService = AppGlobal.Services.TokenService;

namespace WebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IUserProfileService _userProfileSvc;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppSettings _appSettings;

        public AuthController(IUserProfileService _userProfileSvc, TokenService tokenService, IHttpContextAccessor contextAccessor, IOptions<AppSettings> appSettings)
        {
            _contextAccessor = contextAccessor;
            this._userProfileSvc = _userProfileSvc;
            _tokenService = tokenService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Logout))]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Logout()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(_appSettings.SessionConfig.Auth.token);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(AppConstants.CookieSocialToken);
            if (_contextAccessor.HttpContext != null)
            {
                await _contextAccessor.HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(GenResponse<UserLoginResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register(UserModelCreateDto user)
        {
            var objResp = await this._userProfileSvc.RegisterUser(user);
            if (objResp.IsSuccess && !String.IsNullOrWhiteSpace(objResp.Result?.Email))
            {
                objResp.Result.token = _tokenService.CreateAppToken(new AppUserIdentity() { DisplayName = $"{objResp.Result.FirstName} {objResp.Result.LastName}", Email = objResp.Result.Email, Guid = objResp.Result.Guid, Roles = [.. objResp.Result.Roles] }, out List<Claim> userClaims);
                return Ok(objResp);
            }
            return Ok(objResp); ;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(GenResponse<UserLoginResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(UserLoginDto user)
        {
            var objResp = await _userProfileSvc.Login(user);
            if (objResp.IsSuccess && !String.IsNullOrWhiteSpace(objResp.Result.Email))
            {
                AppUserIdentity userInfo = new AppUserIdentity() { DisplayName = $"{objResp.Result.FirstName} {objResp.Result.LastName}", Email = objResp.Result.Email, Guid = objResp.Result.Guid, Roles = [.. objResp.Result.Roles], Id = objResp.Result.Id };
                objResp.Result.token = _tokenService.CreateAppToken(userInfo, out List<Claim> userClaims, 15 * 4 * 24);
                if (objResp.Result.token == null)
                {
                    return StatusCode(objResp.StatCode, GenResponse<UserLoginResponse>.Failed("Unable to generate token. Kindly retry"));
                }
                if (userClaims != null && userClaims.Count > 0)
                {
                    ClaimsIdentity userIdentity = new ClaimsIdentity(userClaims, DefaultAuthenticationTypes.ApplicationCookie);
                    if (_contextAccessor.HttpContext != null)
                    {
                        await _contextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(userIdentity));
                    }
                }

                _contextAccessor.HttpContext?.Response.Cookies.Append(_appSettings.SessionConfig.Auth.token, objResp.Result.token, new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(_appSettings.SessionConfig.Auth.ExpireMinutes),
                    HttpOnly = _appSettings.SessionConfig.Auth.HttpOnly,
                    Secure = _appSettings.SessionConfig.Auth.Secure,
                    IsEssential = _appSettings.SessionConfig.Auth.IsEssential,
                    SameSite = SameSiteMode.None
                });
                // _contextAccessor.HttpContext.Request.Cookies.TryGetValue(AppConstants.CookieUserId, out string cookieValue);
                // AppSessionManager.UpdateUserSessionData(_contextAccessor.HttpContext, GenResponse<AppUserIdentity>.Success(userInfo),cookieValue);
            }
            return StatusCode(objResp.StatCode, objResp);
        }


    }
}
