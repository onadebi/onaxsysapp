using AppCore.Domain.AppCore.Config;
using AppCore.Domain.AppCore.Models;
using AppCore.Persistence;
using AppGlobal.Services.DbAccess;
using Microsoft.Extensions.Logging;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using OnaxTools.Services.StackExchangeRedis.Interface;

namespace AppCore.Services.Common;

public class ResourceAccessService : IResourceAccessService
{
    private readonly ILogger<ResourceAccessService> _logger;
    private readonly AppDbContext _context;
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly ICacheService _cacheService;

    public ResourceAccessService(ILogger<ResourceAccessService> logger, AppDbContext context,
        ISqlDataAccess sqlDataAccess, ICacheService cacheService)
    {
        _logger = logger;
        _context = context;
        _sqlDataAccess = sqlDataAccess;
        _cacheService = cacheService;
    }

    public async Task<GenResponse<ResourceAccess?>> GetResourcePermissionsByResourceName(string resourceName)
    {
        GenResponse<ResourceAccess?> objResp = new();
        try
        {
            var cachedResourcePermissions = await _cacheService.GetData<GenResponse<ResourceAccess?>>($"ResourceAccess_{resourceName}");
            if (cachedResourcePermissions != null && cachedResourcePermissions.IsSuccess && cachedResourcePermissions.Result != null)
            {
                objResp = cachedResourcePermissions;
            }
            else
            {
                IEnumerable<ResourceAccess> objResult = await _sqlDataAccess.GetData<ResourceAccess, dynamic>($"SELECT * FROM {SchemaConstants.AuthSchema}.\"{nameof(ResourceAccess)}\" ra WHERE ra.\"ResourceFullName\" = @ResourceFullName", new { ResourceFullName = resourceName });
                if (objResult != null && objResult.Any())
                {
                    objResp.Result = objResult.FirstOrDefault();
                    objResp.IsSuccess = true;
                    objResp.StatCode = (int)StatusCodeEnum.OK;

                    _ = _cacheService.SetData<GenResponse<ResourceAccess?>>($"ResourceAccess_{resourceName}", objResp, 60 * 5);
                }
                else
                {
                    objResp.Result = null;
                    objResp.Error = "Resource not found";
                    objResp.IsSuccess = false;
                    objResp.StatCode = (int)StatusCodeEnum.NotFound;
                }

            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[UserServiceRepository][GetUserWIthRoleByUserId] {ex.Message}");
            objResp.Error = "An error occurred while processing your request";
            objResp.IsSuccess = false;
            objResp.StatCode = (int)StatusCodeEnum.ServerError;
        }
        return objResp;
    }


    public async Task<GenResponse<bool>> UserHasResourceAccess(string resourceName, string[] userRoles)
    {
        GenResponse<bool> objResp = new();
        GenResponse<ResourceAccess?> resourceAccess = await GetResourcePermissionsByResourceName(resourceName);
        if (resourceAccess.IsSuccess && resourceAccess.Result != null)
        {
            string[] allowedRoles = resourceAccess.Result.AllowedRoles.Split(",");
            objResp.IsSuccess = objResp.Result = userRoles.Any(x => allowedRoles.Contains(x, StringComparer.OrdinalIgnoreCase));
            objResp.StatCode = objResp.IsSuccess ? (int)StatusCodeEnum.OK: (int)StatusCodeEnum.Unauthorized;
        }
        else
        {
            objResp.Result = false;
            objResp.IsSuccess = false;
            objResp.StatCode = (int)StatusCodeEnum.NotFound;
        }
        return objResp;
    }




}


public interface IResourceAccessService
{
    Task<GenResponse<ResourceAccess?>> GetResourcePermissionsByResourceName(string resourceName);
    Task<GenResponse<bool>> UserHasResourceAccess(string resourceName, string[] userRoles);
}
