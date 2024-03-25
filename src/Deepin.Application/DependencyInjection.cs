using Deepin.Application.Queries;
using Deepin.Application.Services;
using Deepin.Infrastructure.EventBus;
using Deepin.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Deepin.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {

        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
        });
        services.AddAutoMapper(assembly);
        services
            .AddQueries()
            .AddServices()
            .AddValidators();

        services.AddEventBus(configuration, assembly);

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var assembily = typeof(DependencyInjection).Assembly;
        var rabbitMQSection = configuration.GetSection("RabbitMQ");
        if (rabbitMQSection.Exists())
        {
            services.AddRabbitMQEventBus(rabbitMQSection.Get<RabbitMqOptions>(), assemblies);
        }
        else
        {
            services.AddInMemoryEventBus(assembily);
        }
        return services;
    }
    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<IChatQueries, ChatQueries>();
        services.AddScoped<IMessageQueries, MessageQueries>();
        services.AddScoped<IFileQueries, FileQueries>();
        return services;
    }
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IFileService, FileService>();
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services;
    }
}
