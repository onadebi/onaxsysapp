using AppCore.Domain.AppCore.Models;
using AppCore.Persistence;
using AppGlobal.Config;
using AppGlobal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using System.Runtime.CompilerServices;

namespace AppGlobal.Services;
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly AppDbContext _context;
    private readonly ISocialAuthService _socialAuthService;

    public AuthService(ILogger<AuthService> logger, AppDbContext context, ISocialAuthService socialAuthService)
    {
        _logger = logger;
        _context = context;
        _socialAuthService = socialAuthService;
    }

    public async Task<GenResponse<UserLoginResponse?>> RegisterUser(UserModelCreateDto user, [CallerMemberName] string? caller = null, CancellationToken ct = default)
    {
        if (user == null)
        {
            return GenResponse<UserLoginResponse?>.Failed("Invalid fields passed.");
        }
        if (user.Password != user.ConfirmPassword)
        {
            return GenResponse<UserLoginResponse?>.Failed("Passwords don't match.");
        }
        user.Email = user.Email.ToLower();
        UserLoginResponse? objResp = null;
        try
        {
            var isExist = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Email == user.Email, ct);

            var userProfileParams = new UserProfile()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email.ToLower(),
                Username = user.Email.ToLower(),
                DisplayName = $"{user.FirstName} {user.LastName}",
                IsSocialLogin = user.SocialLogin.IsSocialLogin,
                UserProfileImage = user.UserProfileImage
            };

            if (isExist != null)
            {
                if (isExist.IsSocialLogin && user.SocialLogin.IsSocialLogin && user.SocialLogin != null && !string.IsNullOrWhiteSpace(user.SocialLogin.SocialLoginAppName))
                {
                    userProfileParams.SocialLoginPlatform = user.SocialLogin.SocialLoginAppName;
                    //TODO: Call Service to validate token
                    GenResponse<bool> socialAuthResp = new();
                    if (user.SocialLogin.SocialLoginAppName.Equals(SocialLoginPlatform.Clerk, StringComparison.InvariantCultureIgnoreCase))
                    {
                        socialAuthResp = await _socialAuthService.ClerkAuthIsValid(user.SocialLogin.token);
                    }
                    if (socialAuthResp.IsSuccess)
                    {
                        objResp = new UserLoginResponse()
                        {
                            Email = user.Email.ToLower(),
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Roles = [..isExist.Roles],
                            Guid = isExist.Guid,
                            Id = isExist.Id,
                            SocialLogin = new()
                            {
                                IsSocialLogin = true,
                                SocialLoginAppName = SocialLoginPlatform.Clerk,
                                token = user.SocialLogin.token
                            }
                        };
                        #region profile updates
                        isExist.UserProfileImage = !string.IsNullOrWhiteSpace(user.UserProfileImage) ? user.UserProfileImage : isExist.UserProfileImage;
                        isExist.LastLoginDate = DateTime.UtcNow;
                        _ = _context.ChangeTracker.HasChanges() ? await _context.SaveChangesAsync(ct) : 0;
                        #endregion
                        return GenResponse<UserLoginResponse?>.Success(objResp, StatusCodeEnum.Created, "Successfully logged in!");
                    }
                }
                return GenResponse<UserLoginResponse?>.Failed("Another user registered with this email already exists.");
            }
            if (!user.SocialLogin.IsSocialLogin)
            {
                if (user.Password == user.ConfirmPassword)
                {
                    userProfileParams.Password = OnaxTools.Cryptify.EncryptSHA512(user.Password);
                }
                else
                {
                    return GenResponse<UserLoginResponse?>.Failed("Invalid registration. Passwords don't match.");
                }
            }
            else
            {
                userProfileParams.SocialLoginPlatform = user.SocialLogin.SocialLoginAppName;
                //TODO: Call Service to validate token
                GenResponse<bool> socialAuthResp = new();
                if (user.SocialLogin.SocialLoginAppName == SocialLoginPlatform.Clerk)
                {
                    socialAuthResp = await _socialAuthService.ClerkAuthIsValid(user.SocialLogin.token);
                }
                if (!socialAuthResp.IsSuccess)
                {
                    return GenResponse<UserLoginResponse?>.Failed("Invalid registration token.");
                }
            }


            var regUser = await _context.UserProfiles.AddAsync(userProfileParams);

            var objSave = await _context.SaveChangesAsync();
            if (objSave > 0)
            {
                objResp = new UserLoginResponse()
                {
                    Email = user.Email.ToLower(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = userProfileParams.Roles != null ? [.. userProfileParams.Roles] : [],
                    Guid = userProfileParams.Guid,
                    Id = userProfileParams.Id
                };
                //string confirmationToken = Guid.NewGuid().ToString();
            }
            else
            {
                return GenResponse<UserLoginResponse?>.Failed("Unable to complete. Kindly try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR in [{caller}]: Occured while registering user: " + user.Email);
            return GenResponse<UserLoginResponse?>.Failed("An internal error occured. Kindly try again.");
        }
        return GenResponse<UserLoginResponse?>.Success(objResp, StatusCodeEnum.Created, "Successfully registered!");
    }

    public async Task<GenResponse<UserLoginResponse>> Login(UserLoginDto userLogin, [CallerMemberName] string? caller = null, CancellationToken ct = default)
    {
        try
        {
            userLogin.Email = userLogin.Email.Trim().ToLower();
            var userDetail = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Email == userLogin.Email.ToLower());
            if (userDetail != null)
            {
                bool IsValidPwd = false;
                if (userLogin.SocialLogin.IsSocialLogin && !string.IsNullOrWhiteSpace(userLogin.SocialLogin.token))
                {
                    //TODO: Call Service to validate token
                    GenResponse<bool> socialAuthResp = new();
                    if (userLogin.SocialLogin.SocialLoginAppName == SocialLoginPlatform.Clerk)
                    {
                        socialAuthResp = await _socialAuthService.ClerkAuthIsValid(userLogin.SocialLogin.token);
                    }
                    if (!socialAuthResp.IsSuccess)
                    {
                        return GenResponse<UserLoginResponse>.Failed("Invalid registration token.");
                    }
                }
                else if ((string.IsNullOrWhiteSpace(userDetail.Password) == false && string.IsNullOrWhiteSpace(userLogin.Password)) || userLogin.SocialLogin.IsSocialLogin == false)
                {
                    IsValidPwd = OnaxTools.Cryptify.EncryptSHA512(userLogin.Password).Equals(userDetail.Password);
                    if (!IsValidPwd)
                    {
                        return GenResponse<UserLoginResponse>.Failed("Invalid email/password supplied.", StatusCodeEnum.NotFound);
                    }
                }
                if (!userDetail.IsDeactivated || userDetail.IsDeleted)
                {
                    return GenResponse<UserLoginResponse>.Failed("User account is currently deactivated or deleted.", StatusCodeEnum.Forbidden);
                }
                else
                {
                    UserLoginResponse result = new UserLoginResponse()
                    {
                        Email = userDetail.Email,
                        FirstName = userDetail.FirstName,
                        LastName = userDetail.LastName,
                        Roles = userDetail.Roles != null ? [.. userDetail.Roles] : [],
                        Guid = userDetail.Guid,
                        Id = userDetail.Id
                    };
                    return GenResponse<UserLoginResponse>.Success(result);
                }
            }
            else
            {
                return GenResponse<UserLoginResponse>.Failed("User account not found or has been deleted.", StatusCodeEnum.NotFound);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR in [{caller}][Login]");
            return GenResponse<UserLoginResponse>.Failed("Internal error! Kindly retry.");
        }
    }


}


public interface IAuthService
{
    Task<GenResponse<UserLoginResponse?>> RegisterUser(UserModelCreateDto user, [CallerMemberName] string? caller = null, CancellationToken ct = default);
    Task<GenResponse<UserLoginResponse>> Login(UserLoginDto userLogin, [CallerMemberName] string? caller = null, CancellationToken ct = default);
}