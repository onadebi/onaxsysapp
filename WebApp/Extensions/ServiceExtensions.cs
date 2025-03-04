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
using AppGlobal.Services.PubSub;
using AppGlobal.Config.Communication;
using System.Text.Json;
using AppCore.Config;
using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
using OnaxTools.Enums.Http;
using AppGlobal.Services.Logger;
using AppCore.Services.Helpers;
using Microsoft.Extensions.Primitives;
using AppCore.Services.Common;

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

        string BlobStorageConstring = Environment.GetEnvironmentVariable("BlobStorageConstring", EnvironmentVariableTarget.Process) ?? string.Empty;
        string BlobReadAccessYear2099 = Environment.GetEnvironmentVariable("BlobReadAccessYear2099", EnvironmentVariableTarget.Process) ?? string.Empty;
        string BlobStoragePath = Environment.GetEnvironmentVariable("BlobStoragePath", EnvironmentVariableTarget.Process) ?? string.Empty;
        string YoutubeApiKeyEnv = Environment.GetEnvironmentVariable("YoutubeApiKeyEnv", EnvironmentVariableTarget.Process) ?? string.Empty;
        string GeminiApiKey = Environment.GetEnvironmentVariable("GeminiApiKeyEnv", EnvironmentVariableTarget.Process) ?? string.Empty;

        services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        services.Configure<ScriptsConfig>(builder.Configuration.GetSection(nameof(ScriptsConfig)));
        services.Configure<SessionConfig>(builder.Configuration.GetSection(nameof(SessionConfig)));
        services.Configure<EmailConfig>(builder.Configuration.GetSection(nameof(EmailConfig)));

        // Globally postmodify the property values of certain fields in AppSettings to read from environment variables
        builder.Services.PostConfigure<AppSettings>(options =>
        {
            options.AzureBlobConfig.BlobStorageConstring = BlobStorageConstring;
            options.AzureBlobConfig.BlobReadAccessYear2099 = BlobReadAccessYear2099;
            options.AzureBlobConfig.BlobStoragePath = BlobStoragePath;
            options.ExternalAPIs.GeminiApi.GeminiApiApiKey = GeminiApiKey;
            options.ExternalAPIs.YoutubeApi.YoutubeApiKey = YoutubeApiKeyEnv;
            options.StartUpAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ??  builder.Environment.ApplicationName;

            #region SpeechSynthesis
            options.SpeechSynthesis.SpeechKey = Environment.GetEnvironmentVariable("SpeechKey", EnvironmentVariableTarget.Process) ?? string.Empty;
            options.SpeechSynthesis.SpeechEndpoint = Environment.GetEnvironmentVariable("SpeechEndpoint", EnvironmentVariableTarget.Process) ?? string.Empty;
            options.SpeechSynthesis.SpeechLocation = Environment.GetEnvironmentVariable("SpeechLocation", EnvironmentVariableTarget.Process) ?? string.Empty;
            #endregion
        });

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

        services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(dbConstring);
        }));
        services.AddHangfireServer();

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
        builder.Services.AddHttpClient(nameof(AppSettings.ExternalAPIs.GeminiApi), (svcProvider, c) =>
        {
            string geminiUrl = svcProvider.GetRequiredService<IOptions<AppSettings>>().Value.ExternalAPIs.GeminiApi.Url + GeminiApiKey;
            c.BaseAddress = new Uri(geminiUrl);
        });

        builder.Services.AddHttpClient(nameof(AppSettings.ExternalAPIs.YoutubeApi), (svcProvider, c) =>
        {
            string youTubeUrl = svcProvider.GetRequiredService<IOptions<AppSettings>>().Value.ExternalAPIs.YoutubeApi.Url;
            c.BaseAddress = new Uri(youTubeUrl);
        });

        #region Swagger authentication
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OnaxApp APIs",
                Version = "1.0.0",
                Contact = new OpenApiContact { Email = "", Name = "OnaxApp" }
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Scheme = "Bearer ",
                Description = "The access key required to access resources on this service. Example: {Bearer, SGE35HWE5EW5363256HERH }"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id= "Bearer",
                            Type= ReferenceType.SecurityScheme
                        }
                    }, new List<string>()
                }
            });
        });
        #endregion

        services.AddAuthentication(opt =>
        {
            #region for Cookie based MVC controller routes authentication
            opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            #endregion


            #region FOR jwt
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            #endregion
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, x =>
        {
            x.SlidingExpiration = true;
            // x.Cookie.Name = "onx_token";
            x.LoginPath = "/home/Login";
            x.AccessDeniedPath = "/home/Login";
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            byte[] key = Array.Empty<byte>();
            key = Encoding.UTF8.GetBytes(encryptionKey);
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
                    string tokenName = builder.Configuration.GetValue<string>(tokenLocation) ?? "onx_token";
                    context.Token = context.Request.Cookies[key: tokenName];
                    #region USe Bearer Token in the absence of Cookie auth
                    if (context.Token == null)
                    {
                        if (context.Request.Headers.TryGetValue("Authorization", out StringValues HeaderAuth))
                        {
                            context.Token = HeaderAuth.ToString().Replace("Bearer ", "");
                            //context.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        }
                    }
                    #endregion
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Token is valid, you can perform additional validation here if needed
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var excepTYpe = context.Exception.GetType();
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Status", "expired");
                    }
                    else if (context.Exception.GetType() == typeof(SecurityTokenSignatureKeyNotFoundException))
                    {
                        context.Response.Headers.Append("Token-Status", "invalid token");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    //if (context.AuthenticateFailure == null)
                    //{
                    //    return Task.CompletedTask;
                    //}
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        message = "Unauthorized",
                        error = context.AuthenticateFailure?.Message,
                        statCode = (int)StatusCodeEnum.Unauthorized
                    });
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
                policy.WithOrigins("http://localhost:5050", "http://localhost:3000", "https://localhost:4500", "https://apis.google.com", "https://accounts.google.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
            });
        });
        #region OLD WAY OF ADDING LOGGER with Activator.CreateInstance
        //builder.Services.AddScoped(typeof(IAppLogger<>), (serviceProvider) =>
        //{
        //    var mongoDataAccess = serviceProvider.GetRequiredService<IMongoDataAccess>();
        //    var appSessionContext = serviceProvider.GetRequiredService<IAppSessionContextRepository>();
        //    var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();

        //    // Get the service type being requested
        //    Type serviceType = serviceProvider.GetType().GetGenericArguments()[0];

        //    // Create the concrete logger type
        //    var loggerType = typeof(AppLogger<>).MakeGenericType(serviceType);

        //    var instance = Activator.CreateInstance(
        //        loggerType,
        //        mongoDataAccess,
        //        appSettings.Value.AppName,
        //        appSessionContext);
        //    return instance ?? throw new InvalidOperationException($"Failed to create instance of {loggerType.Name}");
        //});
        #endregion

        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped(typeof(IPollyService<,>), typeof(PollyService<,>));
        services.AddScoped<IPostCategoryService, PostCategoryService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ISocialAuthService, SocialAuthService>();
        services.AddScoped<IResourceAccessService, ResourceAccessService>();
        services.AddScoped<ISpeechService, SpeechService>();
        services.AddScoped<IAppSessionContextRepository, AppSessionContextRepository>();
        services.AddScoped<IFileManagerHelperService, FileManagerHelperService>();

        builder.Services.AddScoped<IGeminiService>((svcProv) =>
        {
            return new GeminiService(svcProv.GetRequiredService<IHttpClientFactory>()
                , svcProv.GetRequiredService<IOptions<AppSettings>>(), svcProv.GetRequiredService<IMapper>());
        });
        builder.Services.AddScoped<IYouTubeService>((svcProv) =>
        {
            return new YouTubeService(svcProv.GetRequiredService<IHttpClientFactory>()
                , svcProv.GetRequiredService<IOptions<AppSettings>>(), svcProv.GetRequiredService<IMapper>());
        });




        services.AddScoped<IMessageService, MessageService>();

        services.AddSingleton<IMessageBrokerService>((svcP) =>
            new MessageBrokerService(new RabbitMQ.Client.ConnectionFactory { Uri = new Uri(rabbitMqConstring) }, svcP.GetRequiredService<IOptions<AppSettings>>()
            , Factories<MessageBrokerService>.AppLoggerFactory(svcP.GetRequiredService<IConfiguration>()))
        );
        //services.AddScoped<IUserServiceRepository, UserServiceRepository>();
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


