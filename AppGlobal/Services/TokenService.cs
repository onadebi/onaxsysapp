using AppGlobal.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace AppGlobal.Services;
public class TokenService
{
    private readonly string _encryptionKey;
    private readonly AppSettings _appSettings;
    public TokenService(string EncryptionKey, IOptions<AppSettings> sessionConfig)
    {
        _encryptionKey = EncryptionKey;
        _appSettings = sessionConfig.Value;
    }


    public string? CreateAppToken(AppUserIdentity user, out List<Claim> userClaims, int expireInMins = 15)
    {
        string? objResp = null;
        var claims = new List<Claim>
            {
                new Claim("display_name", user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim("nameid", user.Guid),
                new Claim(ClaimTypes.Sid, $"{user.Id}"),
                new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(user.Roles), JsonClaimValueTypes.JsonArray)
            };
        //if (user.Roles != null && user.Roles.Count >= 0)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(user.Roles), JsonClaimValueTypes.JsonArray));
        //}
        byte[] keBytes = System.Text.Encoding.UTF8.GetBytes(_encryptionKey);
        var key = new SymmetricSecurityKey(keBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expireInMins),
            SigningCredentials = creds,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var token = tokenHandler.CreateToken(tokenDescriptor);
            objResp = tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex, $"[TokenService][{nameof(CreateAppToken)}]");
        }
        userClaims = claims;
        return objResp;
    }



    public GenResponse<AppUserIdentity> ValidateToken(HttpContext context, bool refreshBeforeExpiry = false, [CallerMemberName] string callerMemberName = "")
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[TokenService][ValidateToken] Caller: ***{callerMemberName}***");
#endif
        var objResp = new GenResponse<AppUserIdentity>();
        if (context == null)
        {
            return GenResponse<AppUserIdentity>.Failed("Invalid/Expired token credentials");
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_encryptionKey);

        try
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
            context.Request.Cookies.TryGetValue(_appSettings.SessionConfig.Auth.token, out string? cookieValue);
            if (authHeader != null || !string.IsNullOrWhiteSpace(cookieValue))
            {
                string authToken = !string.IsNullOrWhiteSpace(cookieValue) ? cookieValue : authHeader!.Split(" ")[1];
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                jwtTokenHandler.ValidateToken(authToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtoken = (JwtSecurityToken)validatedToken;

                if (jwtoken != null)
                {
                    Dictionary<string, string> tokenValues = new Dictionary<string, string>();
                    foreach (var claim in jwtoken.Claims)
                    {
                        if (tokenValues.ContainsKey(claim.Type))
                        {
                            tokenValues[claim.Type] = tokenValues[claim.Type] + $",{claim.Value}";
                        }
                        else
                        {
                            tokenValues.Add(claim.Type, claim.Value);
                        }
                    }
                    if (tokenValues.Any())
                    {
                        var AppUserIdentity = new AppUserIdentity
                        {
                            Email = tokenValues["email"],
                            DisplayName = tokenValues["unique_name"],
                            Guid = tokenValues["nameid"],
                        };
                        string userRoles = tokenValues["role"];
                        AppUserIdentity.Roles = !string.IsNullOrWhiteSpace(userRoles) ? userRoles.Split(',').ToList<string>() : new List<string>();
                        objResp = GenResponse<AppUserIdentity>.Success(AppUserIdentity);
                        #region Sliding Expiration 
                        if (refreshBeforeExpiry)
                        {

                            TimeSpan timeElapsed = DateTime.UtcNow.Subtract(jwtoken.ValidFrom);
                            TimeSpan timeRemaining = jwtoken.ValidTo.Subtract(DateTime.UtcNow);
                            //if more than half of the timeout interval has elapsed.
                            if (timeRemaining < timeElapsed)
                            {
                                var prolongedToken = this.CreateAppToken(objResp.Result, out var _);
                                context.Response.Headers.Remove("Set-Authorization");
                                _ = context.Response.Headers.TryAdd("Set-Authorization", prolongedToken);

                                context.Response.Cookies.Append(_appSettings.SessionConfig.Auth.token, prolongedToken!, new CookieOptions()
                                {
                                    Expires = DateTime.Now.AddMinutes(_appSettings.SessionConfig.Auth.ExpireMinutes),
                                    HttpOnly = _appSettings.SessionConfig.Auth.HttpOnly,
                                    Secure = _appSettings.SessionConfig.Auth.Secure,
                                    IsEssential = _appSettings.SessionConfig.Auth.IsEssential,
                                    SameSite = SameSiteMode.None
                                });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
                    }
                }
            }
            else { objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials"); }
        }
        catch (SecurityTokenExpiredException ex)
        {
            OnaxTools.Logger.LogException(ex, "[CommonHelpers][ValidateJwt]");
            objResp = GenResponse<AppUserIdentity>.Failed("Expired token");
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex, "[CommonHelpers][ValidateJwt]");
            objResp = GenResponse<AppUserIdentity>.Failed("Invalid/Expired token credentials");
        }
        return objResp;
    }





}
