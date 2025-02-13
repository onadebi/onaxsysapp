using AppGlobal.Config;
using AppGlobal.Models.Logger;
using AppGlobal.Services.DbAccess;
using Microsoft.Extensions.Configuration;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using OnaxTools.Enums.Http;

namespace AppGlobal.Services.Logger;

public class AppLogger<T> : IAppLogger<T> where T : class
{
    private readonly string _contextName;
    private readonly IMongoDataAccess _mongoDataAccess;
    private readonly string _appName;
    private readonly IAppSessionContextRepository _appSessionContext;
    public AppLogger(IMongoDataAccess mongoDataAccess, IConfiguration configuration, IAppSessionContextRepository appSessionContext)
    {
        _contextName = typeof(T).Name; //FullName;
        _mongoDataAccess = mongoDataAccess;
        _appName = configuration["AppName"] ?? "OnaxApp";
        _appSessionContext = appSessionContext;
    }

    public async Task<GenResponse<string>> LogInformationAsync(string message, string? operationBy = null,bool IsSuccessfulOperation = true)
    {
        GenResponse<string> objResp = new();
        AppActivityLog log = new()
        {
            AppName = _appName,
            CallerMemberName = _contextName,
            MessageData = message,
            Operation = "INFO",
            IsSuccessfulOperation = IsSuccessfulOperation,
            OperationBy = string.IsNullOrWhiteSpace(operationBy) ? AppConstants.AppSystem : operationBy,
            CreatedAt = DateTime.UtcNow
        };
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return GenResponse<string>.Failed("Invalid message");
            }
            AppSessionData<AppUserIdentity> user = _appSessionContext.GetUserDataFromSession();
            if (user != null && user.Data != null)
            {
                log.OperationBy = user.Data.DisplayName;
            }
            AppActivityLog objResult = await _mongoDataAccess.InsertRecord<AppActivityLog>(log);
            if (objResult != null)
            {
                objResp.IsSuccess = true; objResp.StatCode = (int)StatusCodeEnum.OK; objResp.Result = log.Id;
            }
            else
            {
                objResp.Error = "Unable to save record. Kindly retry.";
            }
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex);
        }
        return objResp;
    }

    public async Task LogWarningAsync(string message)
    {
        await LogMessageAsync("WARNING", message);
    }

    public async Task LogErrorAsync(string message, Exception? exception = null)
    {
        var errorMessage = exception != null
            ? $"{message} - Exception: {exception.Message}"
            : message;

        await LogMessageAsync("ERROR", errorMessage);
    }

    public async Task LogDebugAsync(string message)
    {
        await LogMessageAsync("DEBUG", message);
    }

    private async Task LogMessageAsync(string level, string message)
    {
        var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] [{_contextName}] - {message}";

        // In a real implementation, you might want to:
        // - Write to a file
        // - Send to a logging service
        // - Store in a database
        // For demonstration, we'll just write to console
        await Console.Out.WriteLineAsync(logEntry);
    }

}

public interface IAppLogger<T> where T : class
{
    Task<GenResponse<string>> LogInformationAsync(string message, string? operationBy = null, bool IsSuccessfulOperation = true);
    Task LogWarningAsync(string message);
    Task LogErrorAsync(string message, Exception? exception = null);
    Task LogDebugAsync(string message);
}