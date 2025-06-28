using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.Security.Claims;

namespace WebApp.Helpers.Filters;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Check if the user is authenticated
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            // Optionally, you can check for specific roles or claims here
            // For example, to allow only users with the "Admin" role:
            // return httpContext.User.IsInRole("Admin");
            IEnumerable<Claim> userRole = httpContext.User.FindAll(claim => claim.Type == ClaimTypes.Role);
            var isInRole = httpContext.User.IsInRole("admin");
            return userRole.ToList().Any(role => role.Value == "admin"); // User is authenticated, allow access
        }
        return false;
    }
}
