using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace AppGlobal.Helpers;

public static class CommonHelpers
{

    public static string GetIp(HttpContext httpContext)
    {
        string ip = httpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? string.Empty;
        if (string.IsNullOrEmpty(ip))
        {
            ip = httpContext.GetServerVariable("REMOTE_ADDR") ?? string.Empty;
        }
        return ip;
    }

    public static bool IsValidJson(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            using (JsonDocument.Parse(text))
            {
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static GenResponse<AppUserIdentity> ValidateJwt(HttpContext context)
    {
        var objResp = new GenResponse<AppUserIdentity>();
        try
        {
            StringValues authValues = context.Request.Headers.Authorization;
            if (StringValues.IsNullOrEmpty(authValues) || !authValues.Any(m => !string.IsNullOrEmpty(m) && m.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase)))
            {
                return GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
            }
            var authHeader = authValues.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
            //context.Request.Headers["Authorization"];//.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
            if (authHeader != null)
            {
                string authToken = !string.IsNullOrWhiteSpace(authHeader) ? authHeader.Split(" ")[1] : string.Empty;
                var jwtoken = new JsonWebTokenHandler().ReadJsonWebToken(authToken);
                Dictionary<string, string> tokenValues = [];
                foreach (var claim in jwtoken.Claims)
                {
                    if (!tokenValues.ContainsKey(claim.Type))
                    {
                        tokenValues.Add(claim.Type, claim.Value);
                    }
                    else
                    {
                        tokenValues[claim.Type] = tokenValues[claim.Type] + "," + claim.Value;
                    }
                }
                if (tokenValues.Any())
                {
                    var appUser = new AppUserIdentity
                    {
                        Email = tokenValues["Email"],
                        DisplayName = tokenValues["display_name"],
                        Guid = tokenValues["guid"],
                        Id = Convert.ToInt32(tokenValues[ClaimTypes.Sid]),
                        ExpiresAt = jwtoken.ValidTo
                    };
                    string userRoles = tokenValues["role"];
                    appUser.Roles = !string.IsNullOrWhiteSpace(userRoles) ? [.. userRoles.Split(',')] : new();
                    objResp = GenResponse<AppUserIdentity>.Success(appUser);
                }
                else
                {
                    objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
                }
            }
            else { objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials"); }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CommonHelpers][ValidateJwt]=> {ex.Message}");
            objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
        }
        return objResp;
    }

}
