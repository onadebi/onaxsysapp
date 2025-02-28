using AppCore.Domain.AppCore.Models;
using AppGlobal.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static AppCore.Persistence.ModelBuilders.SeedData;

namespace AppCore.Persistence.ModelBuilders;

public static class DBInitializers
{
    public static async Task SeedDefaultsData(this IHost host)
    {
        var serviceProvider = host.Services.CreateScope().ServiceProvider;
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var _appsettingsConfig = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        var appDocuments = ApplicationDocuments.GetApplicationDocuments();

        //if (!context.AppDocumentz.Any())
        //{
        //    await context.AppDocumentz.AddRangeAsync(appDocuments);
        //    //await context.SaveChangesAsync();
        //}

        if (!context.UserRoles.Any())
        {
            await context.UserRoles.AddRangeAsync(GetListOfUserRoles());
        }


        List<ResourceAccess> startCheck = GetAllControllerMenu(_appsettingsConfig.Value.StartUpAssemblyName);
        if (startCheck.Count > 0)
        {
            List<ResourceAccess> allResources = await context.ResourceAccess.ToListAsync();

            List<ResourceAccess> resourceDifference = startCheck
                .Where(sc => !allResources.Any(ar => ar.ResourceControllerName == sc.ResourceControllerName && ar.ResourceFunctionName == sc.ResourceFunctionName))
                .ToList();
            if (resourceDifference.Count != 0) { await context.ResourceAccess.AddRangeAsync(resourceDifference); }
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }
}
