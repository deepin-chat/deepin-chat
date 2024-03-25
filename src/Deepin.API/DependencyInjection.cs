using Deepin.API.Extensions;
using Deepin.API.Services;
using Deepin.Application;
using Deepin.Application.Hubs;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

namespace Deepin.API;

internal static class DependencyInjection
{
    public static IServiceCollection AddDeepinChatAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            options.AddPolicy("allow_any",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        services.AddCustomSinalR(configuration);

        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();

        services
            .AddInfrastructure(configuration)
            .AddApplication(configuration)
            .AddAspNetCoreIdentity();
        return services;
    }
    private static IServiceCollection AddAspNetCoreIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        }).AddApiEndpoints()
          .AddEntityFrameworkStores<DeepinDbContext>()
          .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
            options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
        }).AddBearerToken(IdentityConstants.BearerScheme, options =>
        {
            // We have to hook the OnMessageReceived event in order to
            // allow the JWT authentication handler to read the access
            // token from the query string when a WebSocket or 
            // Server-Sent Events request comes in.

            // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
            // due to a limitation in Browser APIs. We restrict it to only calls to the
            // SignalR hub in this code.
            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            // for more information about security considerations when using
            // the query string to transmit the access token.
            options.Events = new Microsoft.AspNetCore.Authentication.BearerToken.BearerTokenEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    private static IServiceCollection AddCustomSinalR(this IServiceCollection services, IConfiguration configuration)
    {
        const string SERVICE_NAME = "deepin";
        var redisConnection = configuration.GetConnectionString("Redis");
        if (string.IsNullOrEmpty(redisConnection))
        {
            services.AddSignalR();
        }
        else
        {
            services.AddDataProtection(opts =>
            {
                opts.ApplicationDiscriminator = SERVICE_NAME;
            })
             .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnection), $"{SERVICE_NAME}-data-protection-keys");

            services.AddSignalR().AddStackExchangeRedis(redisConnection, options => { });
        }
        return services;
    }
    public static WebApplication UseDeepinChatAPI(this WebApplication app)
    {
        app.MigrateDbContext<DeepinDbContext>((db, sp) => { });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("allow_any");
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/hubs/chat");
        app.MapIdentityApi<User>();

        return app;
    }
}
