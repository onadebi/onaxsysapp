using AppCore.Domain.AppCore.Dto;
using AppCore.Persistence;
using AppGlobal.Config;
using AppGlobal.Services.Common.DbAccess;
using AppGlobal.Services.DbAccess;
using AppGlobal.Services;
using Microsoft.Extensions.Options;
using OnaxTools.Services.StackExchangeRedis.Interface;
using WebApp.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AppCore.Services.Blog;
using Microsoft.Extensions.DependencyInjection;
using AppGlobal.Services.PubSub;
using AppGlobal.Config.Communication;
using System.Text.Json;

namespace WebApp.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServiceCollections(this IServiceCollection services, WebApplicationBuilder builder)
    {
        string encryptionKey = Environment.GetEnvironmentVariable("EncryptionKeyEnvVar", EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("AppSettings:Encryption:Key")!;
        string RedisConfig = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("RedisConstring") ?? string.Empty, EnvironmentVariableTarget.Process) ?? builder.Configuration.GetConnectionString("RedisConstring")!;
        string MongoDbCon = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("MongoDbConnect") ?? string.Empty, EnvironmentVariableTarget.Process) ?? builder.Configuration.GetConnectionString("MongoDbConnect")!;
        string dbConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("DBConString") ?? string.Empty, EnvironmentVariableTarget.Process) ?? "Server=localhost;Port=5432;Database=onaxsys;Timeout=120;User Id=postgres;Password=onadebi;";
        string rabbitMqConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetValue<string>("AppSettings:MessageBroker:RabbitMq:ConString") ?? string.Empty, EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("AppSettings:MessageBroker:RabbitMq:ConString")!;

        services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        services.Configure<ScriptsConfig>(builder.Configuration.GetSection(nameof(ScriptsConfig)));
        services.Configure<SessionConfig>(builder.Configuration.GetSection(nameof(SessionConfig)));
        services.Configure<EmailConfig>(builder.Configuration.GetSection(nameof(EmailConfig)));

        //services.AddSingleton<ISqlDataAccess>(new SqlDataAccess(builder.Configuration.GetConnectionString("Default")));
        services.AddSingleton<ISqlDataAccess>((svcProvider) => Factories<SqlDataAccess>.SqlDataAccessService(serviceProvider: svcProvider, conString: dbConstring));
        services.AddScoped<TokenService>((svcProvider) =>
        {
            var _appsettingsConfig = svcProvider.GetRequiredService<IOptions<AppSettings>>();
            return new TokenService(encryptionKey, _appsettingsConfig);
        });
        services.AddSingleton<IMongoDataAccess>((svcProvider) => new MongoDataAccess(MongoDbCon, "onasonic"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbConstring, options => options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds))
            .UseLoggerFactory(LoggerFactory.Create(buildr =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    buildr.AddDebug();
                }
            }))
            );

        //services.AddHangfire(x => x.UsePostgreSqlStorage(dbConstring));
        //services.AddHangfireServer();

        #region REMOVE CACHE FROM IMPACTING STARTUP
        services.AddScoped<ICacheService>((svcProvider) =>
        {
            var appSettings = svcProvider.GetRequiredService<IOptions<AppSettings>>();
            //var inMemCache = svcProvider.GetRequiredService<IMemoryCache>();
            //return new CacheService(RedisConfig, appSettings.Value.AppKey, inMemCache);
            var cxnMultiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect(RedisConfig ?? "");
            return new CacheService(appSettings, cxnMultiplexer);
        });
        // Add in-memory cache [As fallback plan]
        services.AddMemoryCache();
        #endregion



        //services.AddDistributedMemoryCache();
        //services.AddSession((sessionOptions) =>
        //{
        //    sessionOptions.Cookie.Name = "onasc.cookie";
        //    sessionOptions.IdleTimeout = TimeSpan.FromMinutes(60);
        //    //sessionOptions.Cookie.IsEssential = true;
        //});
        //services.AddStackExchangeRedisCache((options) =>
        //{
        //    options.Configuration = RedisConfig;
        //    options.InstanceName = string.Concat(builder.Configuration.GetValue<string>($"{nameof(AppSettings)}:{nameof(AppSettings.AppKey)}"), "Session:");
        //    //options.Configuration = 
        //});


        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddAuthentication(opt =>
        {
            #region for Cookie based MVC controller routes authentication
            opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            #endregion


            #region FOR jwt
            //opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            #endregion
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, x =>
        {
            x.SlidingExpiration = true;
            // x.Cookie.Name = "token";
            x.LoginPath = "/home/Login";
            x.AccessDeniedPath = "/home/Login";
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,options =>
        {
            byte[] key = Array.Empty<byte>();
            if (builder.Environment.IsDevelopment())
            {
                key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Encryption:Key") ?? "");
            }
            else
            {
                key = Encoding.UTF8.GetBytes(encryptionKey);
            }
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                //TODO: Confirm/Add sliding expiration
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            };

            #region Allow for cookies authentication
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    string tokenLocation = $"{nameof(AppSettings)}:{nameof(AppSettings.SessionConfig)}:{nameof(SessionConfig.Auth)}:{nameof(SessionConfig.Auth.token)}";
                    string tokenName = builder.Configuration.GetValue<string>(tokenLocation) ?? "token";
                    context.Token = context.Request.Cookies[key: tokenName];
                    #region USe Bearer Token in the absence of Cookie auth
                    if (context.Token == null)
                    {
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        }
                    }
                    #endregion
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { message = "Unauthorized" });
                    return context.Response.WriteAsync(result);
                }
            };
            #endregion
        })
        ;

        var configMap = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfile());
        });
        var mapper = configMap.CreateMapper();
        services.AddSingleton(mapper);

        services.AddCors(opt =>
        {
            opt.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:4500", "https://localhost:4500")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
            });
        });

        services.AddScoped<IPostCategoryService, PostCategoryService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ISocialAuthService, SocialAuthService>();

        services.AddScoped<IMessageService, MessageService>();

        services.AddSingleton<IMessageBrokerService>((svcP) =>
            new MessageBrokerService(new RabbitMQ.Client.ConnectionFactory { Uri = new Uri(rabbitMqConstring) }, svcP.GetRequiredService<IOptions<AppSettings>>()
            , Factories<MessageBrokerService>.AppLoggerFactory(svcP.GetRequiredService<IConfiguration>()))
        );
        //services.AddScoped<IUserServiceRepository, UserServiceRepository>();
        //services.AddScoped<IAppSessionContextRepository, AppSessionContextRepository>();
        //services.AddScoped<IUserGroupRepository, UserGroupRepository>();

        //services.AddScoped<IMenuRepository, MenuRepository>();

        //services.AddScoped<IAppActivityLogRepository, AppActivityLogRepository>();
        //services.AddScoped<IMessageRepository, MessageServiceRepository>();

        //services.AddScoped<IHMORepository, HMORepository>();
        //services.AddScoped<IPatientRespository, PatientRespository>();
        //services.AddScoped<ICountryRepository, CountryRepository>();
        //services.AddScoped<IStateLocationRepository, StateLocationRepository>();
        //services.AddScoped<IGenderCategoryRespository, GenderCategoryRespository>();
        //services.AddScoped<ISalutationRepository, SalutationRepository>();

        //services.AddScoped<IMedicCompanyRepository, MedicCompanyRepository>();
        //services.AddScoped<IMedicBranchRepository, MedicBranchRepository>();
        //services.AddScoped<IFilesUploadHelperService, FilesUploadHelperService>();

        return services;
    }
}


