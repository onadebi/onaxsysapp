using AppGlobal.Config;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;

namespace AppGlobal.Services;
public class SocialAuthService : ISocialAuthService
{
    private readonly AppSettings _appSettings;

    public SocialAuthService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public async Task<GenResponse<bool>> ClerkAuthIsValid(string? token, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(token))
        {
            return GenResponse<bool>.Failed("Token is invalid.");
        }
        return await Task.Run(()=> GenResponse<bool>.Success(true));
    }
}

public interface ISocialAuthService
{
    Task<GenResponse<bool>> ClerkAuthIsValid(string? token, CancellationToken ct = default);
}
