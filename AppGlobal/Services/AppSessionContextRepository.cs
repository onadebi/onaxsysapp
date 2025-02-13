using AppGlobal.Config;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OnaxTools.Dto.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OnaxTools.Dto.Http;
using AppGlobal.Helpers;

namespace AppGlobal.Services;
public class AppSessionContextRepository : IAppSessionContextRepository
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly TokenService _tokenService;
    private readonly AppSettings _sessionConfig;
    public AppSessionContextRepository(IHttpContextAccessor contextAccessor, IOptions<AppSettings> sessionConfig, TokenService tokenService)
    {
        _contextAccessor = contextAccessor;
        _tokenService = tokenService;
        this._sessionConfig = sessionConfig.Value;
    }

    public AppSessionData<AppUserIdentity> GetUserDataFromSession()
    {
        AppSessionData<AppUserIdentity> objResp = new();
        try
        {
            string? cookieValue = null;
            if (_contextAccessor != null && _contextAccessor.HttpContext != null)
            {

                _contextAccessor.HttpContext.Request.Cookies.TryGetValue(_sessionConfig.SessionConfig.Auth.token, out cookieValue);
                #region First get by claims
                GenResponse<AppUserIdentity> userClaimsData = _tokenService.ValidateToken(_contextAccessor.HttpContext);
                if (userClaimsData.IsSuccess && userClaimsData.Result != null)
                {
                    objResp.Data = userClaimsData.Result;
                    objResp.Email = userClaimsData.Result.Email;
                    return objResp;
                }
                #endregion
            }
            if (String.IsNullOrWhiteSpace(cookieValue))
            {
                return objResp;
            }
            #region Commented out due to session not been configured. Uncomment if session is configured
            //else
            //{
            //    byte[]? userDataFromSession = null;
            //    if (_contextAccessor != null && _contextAccessor.HttpContext != null)
            //    {
            //        _contextAccessor.HttpContext.Session.TryGetValue(key: cookieValue, out userDataFromSession);
            //    }
            //    if (userDataFromSession != null && userDataFromSession.Any())
            //    {
            //        objResp = System.Text.Json.JsonSerializer.Deserialize<AppSessionData<AppUserIdentity>>(Encoding.UTF8.GetString(userDataFromSession)) ?? new AppSessionData<AppUserIdentity>();
            //    }
            //}
            #endregion
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex);
        }
        return objResp;
    }

    public async Task<GenResponse<AppUserIdentity>> GetUserDetails()
    {
        if(_contextAccessor.HttpContext == null)
        {
            return GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
        }
        GenResponse<AppUserIdentity>? tokenUser = CommonHelpers.ValidateJwt(_contextAccessor.HttpContext);
        GenResponse<AppUserIdentity> objResp = tokenUser;
        return await Task.Run(() => objResp);
    }
    public void ClearCurrentUserDataFromSession()
    {
        try
        {
            string? cookieValue = null;
            if (_contextAccessor != null && _contextAccessor.HttpContext != null)
            {
                var authHeader = _contextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
                _contextAccessor.HttpContext.Request.Cookies.TryGetValue(_sessionConfig.SessionConfig.Auth.token, out cookieValue);
                _contextAccessor.HttpContext.Request.Cookies.TryGetValue(AppConstants.CookieUserId, out string? appuser_id);
                if (authHeader != null || !string.IsNullOrWhiteSpace(cookieValue) || !string.IsNullOrWhiteSpace(appuser_id))
                {
                    string authToken = !string.IsNullOrWhiteSpace(cookieValue) ? cookieValue : !string.IsNullOrWhiteSpace(authHeader) ? authHeader.Split(" ")[1] : string.Empty;
                    var jwtTokenHandler = new JwtSecurityTokenHandler();

                    JwtSecurityToken read = jwtTokenHandler.ReadJwtToken(authToken);
                    var userId = read.Claims.FirstOrDefault(m => m.Type.Equals("nameid", StringComparison.InvariantCultureIgnoreCase))?.Value;
                    if (!string.IsNullOrWhiteSpace(appuser_id))
                    {
                        _contextAccessor.HttpContext.Session.Remove(appuser_id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex);
        }
    }
}




public interface IAppSessionContextRepository
{
    AppSessionData<AppUserIdentity> GetUserDataFromSession();
    void ClearCurrentUserDataFromSession();
}