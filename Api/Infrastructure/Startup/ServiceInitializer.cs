namespace url_shortener.Api.Infrastructure.Startup;

using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using url_shortener.Api.Infrastructure.Configurations;
using url_shortener.Api.Infrastructure.Persistence;

public static partial class ServiceInitializer
{
    private static IConfiguration _configuration { get; set; }

    private static string _policyName = "CorsPolicy";

    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        _configuration = configuration;

        RegisterEnv();
        RegisterDatabase(services);
        RegisterControllers(services);
        RegisterSwagger(services);
        RegisterCustomDependencies(services);
        RegisterCors(services);
        RegisterAutoMapper(services);
        RegisterCache(services);

        return services;
    }

    private static void RegisterDatabase(IServiceCollection services)
    {
        var connectionString = _configuration["ConnectionStrings:UrlShortenerDb"];
        if (connectionString is null)
        {
            throw new ApplicationException("Database connection string not found");
        }

        services.AddDbContext<AppDbContext>(
            options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            },
            ServiceLifetime.Transient);
    }

    private static void RegisterControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
    }

    private static void RegisterCustomDependencies(IServiceCollection services)
    {
        services.AddServices("url_shortener.Api.Application.Services", Assembly.GetExecutingAssembly());
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });
        });
    }

    private static void RegisterCors(IServiceCollection services)
    {
        var frontendUrl = _configuration["Application:FrontendUrl"];
        if (frontendUrl is null)
        {
            throw new ApplicationException("Cors URL not found");
        }

        services.AddCors(opt => opt.AddPolicy(_policyName, policy =>
            policy
                .SetIsOriginAllowed(_ => true)
                // .WithOrigins(frontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()));
    }

    private static void RegisterAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ModelToDTOMapping));
    }

    private static void RegisterCache(IServiceCollection services)
    {
        var redisConnectionString = _configuration["ConnectionStrings:UrlShortenerRedis"];
        if (redisConnectionString is null)
        {
            throw new ApplicationException("Redis connection string not found");
        }

        services.AddStackExchangeRedisCache(o =>
        {
            o.InstanceName = "url-shortener-redis";
            o.Configuration = redisConnectionString;
        });
    }

    private static void RegisterEnv()
    {
        Env.Initialize(_configuration);
    }
}
