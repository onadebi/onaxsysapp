﻿using AppGlobal.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;

namespace WebApp.Helpers.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class RoleAuthorizerAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
{
    private readonly string[] allowedRoles;
    private readonly IUserProfileService _userProfileService;
    private readonly TokenService _tokenService;

    public RoleAuthorizerAttribute(IUserProfileService _userProfileService, TokenService tokenService, params string[] roles)
    {
        allowedRoles = roles;
        this._userProfileService = _userProfileService;
        this._tokenService = tokenService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var tokenUser = _tokenService.ValidateToken(context.HttpContext);
        if (tokenUser.IsSuccess)
        {
            string userGuid = tokenUser.Result.Guid;
            var allRoles = _userProfileService.GetUserWithRolesByUserId(userGuid).GetAwaiter().GetResult();
            if (allRoles.IsSuccess && allRoles.Result != null)
            {
                bool isAuthorized = allRoles.Result.Roles.Any(m => allowedRoles.Contains(m, new StringIEqualityComparer()));
                if (!isAuthorized)
                {
                    context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
                }
            }
            else
            {
                context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
            }
        }
        else
        {
            context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
            // context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
        }
    }
}

#region Helpers
public class CustomUnauthorizedResult : JsonResult
{
    public CustomUnauthorizedResult(string message) : base(new CustomError(message))
    {
        StatusCode = StatusCodes.Status401Unauthorized;
    }
}
public class CustomError
{
    public GenResponse<string> Error { get; }

    public CustomError(string message)
    {
        Error = new()
        {
            IsSuccess = false,
            Error = message,
            StatCode = (int)StatusCodeEnum.Unauthorized
        };
    }
}

public class StringIEqualityComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x == null || y == null)
        {
            return x == y;
        }
        return x.Equals(y, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }
}
#endregion