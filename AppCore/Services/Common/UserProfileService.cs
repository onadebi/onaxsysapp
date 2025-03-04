using AppCore.Domain.AppCore.Dto;
using AppCore.Domain.AppCore.Models;
using AppCore.Persistence;
using AppGlobal.Config;
using AppGlobal.Config.Communication;
using AppGlobal.Models;
using AppGlobal.Models.SocialAuth;
using AutoMapper;
using Azure.Core;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using OnaxTools.Enums.Http;
using OnaxTools.Services.StackExchangeRedis.Interface;
using System.Runtime.CompilerServices;
using static Google.Apis.Auth.JsonWebSignature;

namespace AppGlobal.Services;
public class UserProfileService : IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly AppDbContext _context;
    private readonly ISocialAuthService _socialAuthService;
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;
    private readonly ICacheService _cacheService;
    private readonly AppSettings _appSettings;

    public UserProfileService(ILogger<UserProfileService> logger, AppDbContext context, IOptions<AppSettings> appSettings
        , ISocialAuthService socialAuthService, IMapper mapper, IMessageService messageService, ICacheService cacheService)
    {
        _logger = logger;
        _context = context;
        _appSettings = appSettings.Value;
        _socialAuthService = socialAuthService;
        _mapper = mapper;
        _messageService = messageService;
        _cacheService = cacheService;
    }

    public async Task<GenResponse<UserLoginResponse>> RegisterUser(UserModelCreateDto user, [CallerMemberName] string? caller = null, CancellationToken ct = default)
    {
        if (user == null)
        {
            return GenResponse<UserLoginResponse>.Failed("Invalid fields passed.");
        }
        if (user.Password != user.ConfirmPassword)
        {
            return GenResponse<UserLoginResponse>.Failed("Passwords don't match.");
        }
        user.Email = user.Email.ToLower();
        GenResponse<UserLoginResponse> objResp = new();
        try
        {
            var isExist = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Email == user.Email, ct);
            UserProfile userProfileParams = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email.ToLower(),
                Username = user.Email.ToLower(),
                DisplayName = $"{user.FirstName} {user.LastName}",
                IsSocialLogin = false,
                UserProfileImage = user.UserProfileImage
            };
            if (isExist != null)
            {
                return GenResponse<UserLoginResponse>.Failed("Another user registered with this email already exists.");
            }
            if (user.Password == user.ConfirmPassword)
            {
                userProfileParams.Password = OnaxTools.Cryptify.EncryptSHA512(user.Password);
            }
            else
            {
                return GenResponse<UserLoginResponse>.Failed("Invalid registration. Passwords don't match.");
            }
            await _context.UserProfiles.AddAsync(userProfileParams, ct);
            UserApp userApp = new()
            {
                OAuthIdentity = null,
                AppId = _appSettings.AppName.ToLower(),
                UserId = userProfileParams.Guid,
                UserProfile = userProfileParams,
                UserRole = ["user"]
            };
            await _context.UserApps.AddAsync(userApp, ct);

            var objSave = await _context.SaveChangesAsync(ct);
            if (objSave > 0)
            {
                objResp.Result = new UserLoginResponse()
                {
                    Email = user.Email.ToLower(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = [..userApp.UserRole],
                    Guid = userProfileParams.Guid,
                    Id = userProfileParams.Id,
                    Picture = userProfileParams.UserProfileImage
                };
                objResp.StatCode = (int)StatusCodeEnum.Created;
                objResp.Message = "Successfully registered!";
                objResp.IsSuccess = true;

                #region EMAIL CONFIRMATION
                string confirmationToken = Guid.NewGuid().ToString();
                string EmailBody = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ConfirmEmail.html"))
                        .Replace("##token##", confirmationToken)
                        .Replace("##name##", user.FirstName)
                        .Replace("##guid##", objResp.Result.Guid);
                EmailModelDTO emailBody = new ()
                {
                    ReceiverEmail = user.Email,
                    EmailSubject = MessageOperations.ConfirmEmail,
                    EmailBody = EmailBody
                };
                MessageBox msg = new()
                {
                    AppName = _appSettings.AppName,
                    Operation = AppConstants.EmailTemplateConfirmEmail,
                    Channel = "Email",
                    EmailReceiver = user.Email,
                    IsProcessed = false,
                    IsUsed = false,
                    MessageData = confirmationToken,
                    UserId = objResp.Result.Guid,
                    ForQueue = false,
                    ExpiredAt = DateTime.Now.AddMinutes(10)

                };
                var wasSuccessful = await _messageService.InsertNewMessageAndSendMail(emailBody, msg);
                #endregion
            }
            else
            {
                return GenResponse<UserLoginResponse>.Failed("Unable to complete. Kindly try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR in [{caller}]: Occured while registering user: " + user.Email);
            if (!objResp.IsSuccess)
            {
                return GenResponse<UserLoginResponse>.Failed("An internal error occured. Kindly try again.");
            }
        }
        return objResp;
    }

    public async Task<GenResponse<AppUserIdentity>> GetUserWithRolesByUserId(string UserGuid)
    {
        AppUserIdentity? objResp = new AppUserIdentity();
        if (string.IsNullOrWhiteSpace(UserGuid))
        {
            return GenResponse<AppUserIdentity>.Failed("Invalid user details requested");
        }
        try
        {
            var cachedUser = await _cacheService.GetData<AppUserIdentity>($"UserWithRoles_{UserGuid}");
            if (cachedUser != null)
            {
                objResp = cachedUser;
            }
            else
            {
                objResp = await _context.UserProfiles.Include(m => m.UserProfileUserApps)
                .Select(u => new AppUserIdentity
                {
                    DisplayName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Guid = u.Guid,
                    Roles = u.UserProfileUserApps.First((UserApp u) => u.UserId == UserGuid) != null ? u.UserProfileUserApps.First((UserApp u) => u.UserId == UserGuid).UserRole : new List<string>()
                }).FirstOrDefaultAsync(m => m.Guid == UserGuid);

                if (objResp != null)
                {
                    _ = _cacheService.SetData<AppUserIdentity>($"UserWithRoles_{UserGuid}", objResp, 60 * 5);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[UserServiceRepository][GetUserWIthRoleByUserId] {ex.Message}");
            return GenResponse<AppUserIdentity>.Failed($"Sorry, an internal error occured. Kindly try again.");
        }
        return objResp == null ? GenResponse<AppUserIdentity>.Failed($"No profile found this user id.", StatusCodeEnum.NotFound) : GenResponse<AppUserIdentity>.Success(objResp);
    }

    public async Task<GenResponse<UserLoginResponse>> GoogleLogin(string token, [CallerMemberName] string? caller = null, CancellationToken ct = default)
    {
        GenResponse<UserLoginResponse> objResp = new();
        try
        {
            GenResponse<GoogleOAuthResponse> googleOAuthResp = await VerfiyGoogleAuth(token);
            if (!googleOAuthResp.IsSuccess)
            {
                return GenResponse<UserLoginResponse>.Failed("Invalid google token.");
            }
            string userLoginEamil = googleOAuthResp.Result.Email.ToLowerInvariant();
            var userDetail = await _context.UserProfiles.Include(m => m.UserProfileUserApps).FirstOrDefaultAsync(m => m.Email == userLoginEamil);
            if (userDetail != null)
            {
                var userRoles = userDetail.UserProfileUserApps.Count > 0 ? userDetail.UserProfileUserApps.FirstOrDefault(m => m.AppId == _appSettings.AppName.ToLower() && m.OAuthIdentity == SocialLoginPlatform.Google)?.UserRole : ["user"];
                UserLoginResponse result = new()
                {
                    Email = userDetail.Email,
                    FirstName = userDetail.FirstName,
                    LastName = userDetail.LastName,
                    Roles = userRoles == null ? ["user"]: [.. userRoles],
                    Guid = userDetail.Guid,
                    Id = userDetail.Id,
                    Picture = userDetail.UserProfileImage,
                    SocialLogin = new() { IsSocialLogin = true }
                };
                return GenResponse<UserLoginResponse>.Success(result);
            }
            else // Register the user
            {
                GoogleOAuthResponse user = googleOAuthResp.Result;

                var userProfileParams = new UserProfile()
                {
                    FirstName = user.GivenName,
                    LastName = user.FamilyName,
                    Email = user.Email.ToLower(),
                    Username = user.Email.ToLower(),
                    DisplayName = $"{user.GivenName} {user.FamilyName}",
                    IsSocialLogin = true,
                    IsEmailConfirmed = true,
                    UserProfileImage = user.Picture
                };
                await _context.UserProfiles.AddAsync(userProfileParams, ct);
                UserApp userApp = new()
                {
                    OAuthIdentity = SocialLoginPlatform.Google,
                    OAuthGuid = user.JwtId,
                    AppId = _appSettings.AppName.ToLower(),
                    UserId = userProfileParams.Guid,
                    UserProfile = userProfileParams,
                    UserRole = ["user"]
                };
                await _context.UserApps.AddAsync(userApp, ct);
                objResp.IsSuccess = await _context.SaveChangesAsync(ct) > 1;
                if (objResp.IsSuccess)
                {
                    objResp.Result = new UserLoginResponse()
                    {
                        Email = userProfileParams.Email,
                        FirstName = userProfileParams.FirstName,
                        LastName = userProfileParams.LastName,
                        Roles = userProfileParams.UserProfileUserApps.Count > 0 ? [.. userApp.UserRole] : ["user"],
                        Guid = userProfileParams.Guid,
                        Id = userProfileParams.Id,
                        Picture = userProfileParams.UserProfileImage,
                        SocialLogin = new() { IsSocialLogin = true }
                    };
                    objResp.StatCode = (int)StatusCodeEnum.OK;
                }
                else
                {
                    objResp.Error = "Unable to complete registration. Kindly retry.";
                    objResp.StatCode = (int)StatusCodeEnum.ServerError;
                }
            }
        }
        catch (Exception ex)
        {
            objResp.Error = "Internal error! Kindly retry.";
            objResp.Message = ex.Message;
            objResp.StatCode = (int)StatusCodeEnum.ServerError;
            _logger.LogError(ex, $"ERROR in [{caller}][Login]");
        }
        return objResp;
    }


    public async Task<GenResponse<UserLoginResponse>> Login(UserLoginDto userLogin, [CallerMemberName] string? caller = null, CancellationToken ct = default)
    {
        try
        {
            userLogin.Email = userLogin.Email.Trim().ToLower();
            var userDetail = await _context.UserProfiles.Include(m=> m.UserProfileUserApps).FirstOrDefaultAsync(m => m.Email == userLogin.Email.ToLower());
            if (userDetail != null)
            {
                bool IsValidPwd = false;
                if (!string.IsNullOrWhiteSpace(userDetail.Password) && !string.IsNullOrWhiteSpace(userLogin.Password))
                {
                    IsValidPwd = OnaxTools.Cryptify.EncryptSHA512(userLogin.Password).Equals(userDetail.Password);
                    if (!IsValidPwd)
                    {
                        return GenResponse<UserLoginResponse>.Failed("Invalid email/password supplied.", StatusCodeEnum.Unauthorized);
                    }
                }
                if (userDetail.IsDeactivated || userDetail.IsDeleted)
                {
                    return GenResponse<UserLoginResponse>.Failed("User account is currently deactivated or deleted.", StatusCodeEnum.Forbidden);
                }
                else
                {
                    UserLoginResponse result = new()
                    {
                        Email = userDetail.Email,
                        FirstName = userDetail.FirstName,
                        LastName = userDetail.LastName,
                        Roles = userDetail.UserProfileUserApps.Count > 0 ? [.. userDetail.UserProfileUserApps.FirstOrDefault()?.UserRole] : ["user"],
                        Guid = userDetail.Guid,
                        Id = userDetail.Id,
                        Picture = userDetail.UserProfileImage
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

    public async Task<GenResponse<GoogleOAuthResponse>> VerfiyGoogleAuth(string token)
    {
        GenResponse<GoogleOAuthResponse> objResp = new();
        string clientId = Environment.GetEnvironmentVariable(_appSettings.ExternalAPIs.GoogleOAuth.ClientId, EnvironmentVariableTarget.Process) ?? "";
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = [clientId] };
            Google.Apis.Auth.GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            if (!string.IsNullOrWhiteSpace(payload.Email))
            {
                objResp.Result = _mapper.Map<GoogleOAuthResponse>(payload);
                objResp.IsSuccess = true;
                objResp.StatCode = (int)StatusCodeEnum.OK;
            }
            else
            {
                objResp.Error = "Google authentication failed";
                objResp.StatCode = (int)StatusCodeEnum.BadRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while verifying google auth token.");
            objResp.Error = "Google authentication failed";
        }
        return objResp;
    }

}


public interface IUserProfileService
{
    Task<GenResponse<UserLoginResponse>> RegisterUser(UserModelCreateDto user, [CallerMemberName] string? caller = null, CancellationToken ct = default);
    Task<GenResponse<AppUserIdentity>> GetUserWithRolesByUserId(string UserGuid);
    Task<GenResponse<UserLoginResponse>> Login(UserLoginDto userLogin, [CallerMemberName] string? caller = null, CancellationToken ct = default);
    Task<GenResponse<UserLoginResponse>> GoogleLogin(string token, [CallerMemberName] string? caller = null, CancellationToken ct = default);
    Task<GenResponse<GoogleOAuthResponse>> VerfiyGoogleAuth(string token);
}