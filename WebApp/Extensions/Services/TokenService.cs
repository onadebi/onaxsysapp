using AppGlobal.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApp.Extensions.Services;
public class TokenService
{
    private readonly string _encryptionKey;
    private readonly SessionConfig _sessionConfig;

    private const string _applicationMenu_ = $"{nameof(_applicationMenu_)}_Global";

    public TokenService(string EncryptionKey, IOptions<SessionConfig> sessionConfig)
    {
        _encryptionKey = EncryptionKey;
        _sessionConfig = sessionConfig.Value;
    }

    public string CreateToken(AppUserIdentity user, int expireInMins = 15)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("guid", user.Guid),
                new Claim("Id", $"{user.Id}"),
            };
        if (user.Roles != null && user.Roles.Count >= 0)
        {
            claims.Add(new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(user.Roles), JsonClaimValueTypes.JsonArray));
        }

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_encryptionKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expireInMins),
            SigningCredentials = creds,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<GenResponse<AppUserIdentity>> ValidateToken(HttpContext context, bool refreshBeforeExpiry = false)
    {
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
            context.Request.Cookies.TryGetValue(_sessionConfig.Auth.token, out string? cookieValue);
            if (authHeader != null || !string.IsNullOrWhiteSpace(cookieValue))
            {
                string authToken = !string.IsNullOrWhiteSpace(cookieValue) ? cookieValue : authHeader!.Split(" ")[1];
                //TODO: Delete below
                var eventual = GetUserClaims(authToken);
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

                JwtSecurityToken jwtoken = (JwtSecurityToken)validatedToken;

                if (jwtoken != null)
                {
                    Dictionary<string, string> tokenValues = [];
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
                        var appUser = new AppUserIdentity
                        {
                            Email = tokenValues["email"],
                            DisplayName = tokenValues["unique_name"],
                            Guid = tokenValues["guid"],
                            Id = Convert.ToInt64(tokenValues["Id"]),
                        };
                        tokenValues.TryGetValue("role", out string? userRoles);
                        appUser.Roles = !string.IsNullOrWhiteSpace(userRoles) ? userRoles.Split(',').ToList<string>() : new List<string>();
                        objResp = GenResponse<AppUserIdentity>.Success(appUser);
                        #region Sliding Expiration 
                        if (refreshBeforeExpiry)
                        {

                            TimeSpan timeElapsed = DateTime.UtcNow.Subtract(jwtoken.ValidFrom);
                            TimeSpan timeRemaining = jwtoken.ValidTo.Subtract(DateTime.UtcNow);
                            //if more than half of the timeout interval has elapsed.
                            if (timeRemaining < timeElapsed)
                            {
                                var prolongedToken = this.CreateToken(objResp.Result, _sessionConfig.Auth.ExpireMinutes);
                                context.Response.Headers.Remove("Set-Authorization");
                                context.Response.Headers.Append("Set-Authorization", prolongedToken);

                                context.Response.Cookies.Append(_sessionConfig.Auth.token, prolongedToken, new CookieOptions()
                                {
                                    Expires = DateTime.Now.AddMinutes(_sessionConfig.Auth.ExpireMinutes),
                                    HttpOnly = _sessionConfig.Auth.HttpOnly,
                                    Secure = _sessionConfig.Auth.Secure,
                                    IsEssential = _sessionConfig.Auth.IsEssential,
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
        catch (SecurityTokenExpiredException err)
        {
            await Task.Run(()=>OnaxTools.Logger.LogException(err, "[CommonHelpers][ValidateJwt]"));
            objResp = GenResponse<AppUserIdentity>.Failed("Expired token credentials");
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex, "[CommonHelpers][ValidateJwt]");
            objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
        }
        return objResp;
    }


    public GenResponse<AppUserIdentity> GetUserClaims(string authToken)
    {
        GenResponse<AppUserIdentity> objResp = new() { Result = new() };
        var key = Encoding.ASCII.GetBytes(_encryptionKey);
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

        JwtSecurityToken jwtoken = (JwtSecurityToken)validatedToken;

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
                var appUser = new AppUserIdentity
                {
                    Email = tokenValues["email"],
                    DisplayName = tokenValues["unique_name"],
                    Guid = tokenValues["guid"],
                    Id = Convert.ToInt64(tokenValues["Id"]),
                };
                tokenValues.TryGetValue("role", out string? userRoles);
                appUser.Roles = !string.IsNullOrWhiteSpace(userRoles) ? userRoles.Split(',').ToList<string>() : new List<string>();
                return GenResponse<AppUserIdentity>.Success(appUser);
            }
            else
            {
                objResp = GenResponse<AppUserIdentity>.Failed("Invalid token credentials");
            }
        }
        return objResp;
    }

}
