using AppGlobal.Services.DbAccess;

namespace WebApp.Helpers;
public class Factories<T> where T : class
{
    public static ILogger<T> AppLoggerFactory(IConfiguration AppLogLevel)
    {
        var config = AppLogLevel;
        ILogger<T> logger = LoggerFactory.Create(builder =>
        {
            var logLevel = config.GetSection("Logging");
            builder.AddConfiguration(logLevel);
            builder.AddDebug(); //does all log levels
            builder.AddConsole();
        }).CreateLogger<T>();
        return logger;
    }

    public static ISqlDataAccess SqlDataAccessService(IServiceProvider serviceProvider, string conString)
    {
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        //var config = serviceProvider.GetRequiredService<IConfiguration>();
        //ILogger<SqlDataAccess> logger = Factories<SqlDataAccess>.AppLoggerFactory(config);
        if (env.IsDevelopment())
        {
            string devAppConstring = conString;
            return new SqlDataAccess(conString: devAppConstring);
        }
        else
        {
            return new SqlDataAccess( conString);
        }
    }
}
