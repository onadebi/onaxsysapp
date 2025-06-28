using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AppCore.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Adjust the path to your WebApp's appsettings.json as needed
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("../WebApp/appsettings.Development.json", optional: true)
                .AddJsonFile("../WebApp/appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            //var connectionString = configuration.GetConnectionString("DBConString");
            string dbConstring = Environment.GetEnvironmentVariable(configuration.GetConnectionString("DBConString") ?? string.Empty, EnvironmentVariableTarget.Process) ?? configuration.GetConnectionString("DBConString")!;
            Console.WriteLine("DbCon:: "+dbConstring);
            optionsBuilder.UseNpgsql(dbConstring);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
