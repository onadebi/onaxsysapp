using AppGlobal.Config;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace AppGlobal.Services;
public class TokenService
{
    private readonly string _encryptionKey;
    private readonly TelemetryClient _telemetryClient;
    private readonly AppSettings _appSettings;

    public TokenService(string EncryptionKey, IOptions<AppSettings> sessionConfig, TelemetryClient telemetryClient)
    {
        _encryptionKey = EncryptionKey;
        _telemetryClient = telemetryClient;
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
                new Claim("guid", user.Guid),
                new Claim(ClaimTypes.Sid, $"{user.Id}"),
                new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(user.Roles), JsonClaimValueTypes.JsonArray)
            };
        #region May not be needed
        //if (user.Roles != null && user.Roles.Count >= 0)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(user.Roles), JsonClaimValueTypes.JsonArray));
        //}
        #endregion
        byte[] keBytes = System.Text.Encoding.UTF8.GetBytes(_encryptionKey);
        int keyLength = _encryptionKey.Length;
        _telemetryClient.TrackEvent($"The length of encryption key is [{keyLength}]");
        Console.WriteLine($"The length of encryption key is [{keyLength}]");
        var key = new SymmetricSecurityKey(keBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expireInMins),
            SigningCredentials = creds,
        };
        var tokenHandler = new JsonWebTokenHandler();
        try
        {
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            objResp = tokenHandler.CreateToken(tokenDescriptor);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[CreateTokeError]:::::" + ex.Message);
            _telemetryClient.TrackEvent("[CreateTokeError]:::::" + ex.Message);
            _telemetryClient.TrackException(ex);
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
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.ASCII.GetBytes(_encryptionKey);

        try
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
            context.Request.Cookies.TryGetValue(_appSettings.SessionConfig.Auth.token, out string? cookieValue);
            if (authHeader != null || !string.IsNullOrWhiteSpace(cookieValue))
            {
                string authToken = !string.IsNullOrWhiteSpace(cookieValue) ? cookieValue : authHeader!.Split(" ")[1];
                var jwtTokenHandler = new JsonWebTokenHandler();

                var result = jwtTokenHandler.ValidateTokenAsync(authToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    RequireExpirationTime = true
                }).Result;

                if (result.IsValid && result.SecurityToken is JsonWebToken jwtoken)
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
                            Email = tokenValues[ClaimTypes.Sid],
                            DisplayName = tokenValues["display_name"],
                            Guid = tokenValues["guid"],
                            Id = Convert.ToInt32(tokenValues[ClaimTypes.Sid]),
                            ExpiresAt = jwtoken.ValidTo
                        };
                        string userRoles = tokenValues[ClaimTypes.Role];
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
                else { objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials"); }
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
