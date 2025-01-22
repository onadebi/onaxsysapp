using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static AppCore.Persistence.ModelBuilders.SeedData;

namespace AppCore.Persistence.ModelBuilders;

public static class DBInitializers
 {
     public static async Task SeedDefaultsData(this IHost host)
     {
         var serviceProvider = host.Services.CreateScope().ServiceProvider;
         var context = serviceProvider.GetRequiredService<AppDbContext>();
         var appDocuments = ApplicationDocuments.GetApplicationDocuments();

         if (!context.AppDocumentz.Any())
         {
             await context.AppDocumentz.AddRangeAsync(appDocuments);
             //await context.SaveChangesAsync();
         }

         if (!context.UserRoles.Any())
         {
             await context.UserRoles.AddRangeAsync(GetListOfUserRoles());
             //await context.SaveChangesAsync();
         }

         if (context.ChangeTracker.HasChanges())
         {
             await context.SaveChangesAsync();
         }
     }
 }
