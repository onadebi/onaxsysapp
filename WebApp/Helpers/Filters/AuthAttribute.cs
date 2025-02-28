using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AppGlobal.Services;
using AppCore.Services.Common;
using OnaxTools.Dto.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace WebApp.Helpers.Filters;

public class AuthAttribute : TypeFilterAttribute
{
    public AuthAttribute(params string[] roles) : base(typeof(RoleAuthorizerAttribute))
    {
        Arguments = new object[] {
            roles
        };
    }
}

#region Modified
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class AuthAttributeDynamicOnx : Attribute, IAsyncAuthorizationFilter
{
    private readonly string? _resourceName;

    public AuthAttributeDynamicOnx(string? resourceName = null)
    {
        _resourceName = resourceName;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get the action descriptor to find the controller and action name details
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor != null)
        {
            // Format: "ClassName-MethodName"
            var classNamespace = actionDescriptor.ControllerTypeInfo.Namespace;
            var className = actionDescriptor.ControllerTypeInfo.Name;
            var methodName = actionDescriptor.ActionName;
            var resourceName = string.IsNullOrWhiteSpace(_resourceName) ? $"{classNamespace}#{className}#{methodName}": $"{classNamespace}#{className}#{_resourceName}";

            IAppSessionContextRepository userSessionInfo = context.HttpContext.RequestServices.GetRequiredService<IAppSessionContextRepository>();
            var user = userSessionInfo.GetUserDataFromSession();
            IResourceAccessService resourceAccessService = context.HttpContext.RequestServices.GetRequiredService<IResourceAccessService>();

            // Use the dynamically determined resource name
            GenResponse<bool> hasAccess = await resourceAccessService.UserHasResourceAccess(resourceName, user.Data != null ? [.. user.Data.Roles]: []);

            if (!hasAccess.IsSuccess || !hasAccess.Result)
            {
                context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
            }
        }
        else
        {
            // Fallback if we can't determine the controller/action names
            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
            //context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
        }
    }
}
#endregion