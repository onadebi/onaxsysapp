using AppCore.Persistence.ModelBuilders;
using Hangfire.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using WebApp.Extensions;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region CUSTOM SERVICES AND DI
builder.Services.AddCustomServiceCollections(builder);
#endregion

var app = builder.Build();

try
{
    bool seedDatabase =  builder.Configuration.GetValue<bool>("AppSettings:DatabaseOptions:SeedDatabase");
    if (seedDatabase)
    {
        app.SeedDefaultsData().Wait();
    }
}
catch (Exception ex)
{
    OnaxTools.Logger.LogException(ex);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();


#region Static frontend serving
app.UseDefaultFiles(); // Serve the index.html file by default
string currentDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "portal");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(currentDir),
    RequestPath = ""
});
#endregion

app.Use(async (context, next) =>
{
    var req = context.Request;
    Console.Write($"::[{req.Method}] [${DateTime.Now:yyyy-MM-dd:hh:mm:ss}] request for path: [{req.Path}] || ");
    await next(context);
});
app.Use(async (context, next) =>
{
    var resp = context.Response;
    Console.WriteLine($"Response::[{resp.StatusCode}]");
    await next(context);
});

app.MapHub<AppNotificationHub>("/notificationshub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
 app.MapFallbackToController("Index", "Zone");
app.Run();
