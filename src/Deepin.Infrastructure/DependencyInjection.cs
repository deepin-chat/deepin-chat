using Deepin.Domain.Entities;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.EventBus;
using Deepin.Infrastructure.FileSystem;
using Deepin.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Reflection;

namespace Deepin.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
                .AddDbContext(configuration)
                .AddMongoDbContext(configuration)
                .AddRedisCache(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("Redis");
                })
                .AddFileStorage(configuration);
    }
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var azureBlobSection = configuration.GetSection("AzureBlob");
        if (azureBlobSection.Exists())
        {
            services.AddAzureBlobStorage(azureBlobSection);
        }
        else
        {
            services.AddFileSystemStorage();
        }
        return services;
    }
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfigurationSection azureBlobSection)
    {
        //TODO 
        return services;
    }
    public static IServiceCollection AddFileSystemStorage(this IServiceCollection services)
    {
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        return services;
    }
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = typeof(DeepinDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<DeepinDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgre"), sql =>
            {
                sql.MigrationsAssembly(migrationsAssembly);
                sql.EnableRetryOnFailure(5);
            });
        }, ServiceLifetime.Scoped);

        return services;
    }
    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(s => new MongoClient(configuration.GetConnectionString("Mongo")));
        services.AddSingleton(s => new DocumentDbContext(s.GetRequiredService<MongoClient>(), "deepin"));
        services.AddSingleton(typeof(IDocumentRepository<>), typeof(DocumentRepository<>));
        MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<ChatMessage>(options =>
        {
            options.AutoMap();
            options.MapProperty(p => p.Id)
            .SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId))
            .SetIdGenerator(new MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator());
        });
        MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<FileObject>(options =>
        {
            options.AutoMap();
            options.MapProperty(p => p.Id)
            .SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId))
            .SetIdGenerator(new MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator());
        });
        MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UserSession>(options =>
        {
            options.AutoMap();
            options.MapProperty(p => p.Id)
            .SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId))
            .SetIdGenerator(new MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator());
        });
        return services;
    }
    public static IServiceCollection AddRedisCache(this IServiceCollection services, Action<RedisCacheOptions> actionOptions)
    {
        var options = new RedisCacheOptions();
        actionOptions(options);

        services.AddSingleton(sp =>
        {
            var config = ConfigurationOptions.Parse(options.ConnectionString, true);
            config.ResolveDns = true;
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<ICacheManager>(sp =>
            new RedisCacheManager(sp.GetRequiredService<ConnectionMultiplexer>(), new RedisCacheOptions()));
        return services;
    }

    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumers(assemblies);
            cfg.UsingInMemory((ctx, cfg) =>
            {
                cfg.ConfigureEndpoints(ctx);
            });
        });
        return services;
    }
    public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, RabbitMqOptions mqConfig, params Assembly[] assemblies)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(assemblies);
            config.UsingRabbitMq((context, mq) =>
            {
                mq.Host(mqConfig.HostName, mqConfig.VirtualHost, h =>
                {
                    h.Username(mqConfig.UserName);
                    h.Password(mqConfig.Password);

                });
                mq.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
