using AppGlobal.Config;
using AppGlobal.Services;
using Microsoft.AspNetCore.Http;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.Security.Claims;

namespace WebApp.Helpers.Middleware;

public class AppSessionManager
{
    public readonly RequestDelegate _next;
    public AppSessionManager(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TokenService tokenService, IAppSessionContextRepository appSessionContextRepository)
    {

        var userSession = appSessionContextRepository.GetUserDataFromSession(excludeDetails: true, true);
        context.Request.Cookies.TryGetValue(AppConstants.CookieUserId, out string cookieValue);
        IEnumerable<Claim> userRole = context.User.FindAll(claim => claim.Type == ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(cookieValue) && !userRole.Any())
        {
            // If no cookie or user role, clear session data
            appSessionContextRepository.ClearCurrentUserDataFromSession();
        }
        else if (!string.IsNullOrWhiteSpace(cookieValue) || userRole.Any())
        {
            // If cookie exists but no user role, validate the token
            GenResponse<AppUserIdentity> tokenUser = tokenService.ValidateToken(context);
            if (tokenUser == null || !tokenUser.IsSuccess)
            {
                appSessionContextRepository.ClearCurrentUserDataFromSession();
                return;
            }
        }
        //var objUser = tokenService.ValidateToken(context, refreshBeforeExpiry: true);
        //}
        // Ensure the session is initialized
        //if (context.Session == null)
        //{
        //    context.Session = new Session(context);
        //}
        // Load user data into session
        //var userData = appSessionContextRepository.GetUserDataFromSession();
        if (userSession != null && userSession.Data != null)
        {
            //context.Items["UserData"] = userData.Data;
        }
        // Call the next middleware in the pipeline
        await _next(context);
    }
}
