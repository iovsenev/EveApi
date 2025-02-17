using Eve.Domain.Constants;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.DataBaseAccess.Write;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Infrastructure.DataBase.Repositories.Write;
using Eve.Infrastructure.ExternalServices;
using Eve.Infrastructure.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eve.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddContexts(configuration);
        services.AddProviders();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IReadCategoryRepository, ReadCategoryRepository>();
        services.AddScoped<IReadRegionRepository, ReadRegionRepository>();
        services.AddScoped<ILoadRepository, LoadRepository>();
        services.AddScoped<IMarketReadRepository, MarketGroupRepository>();
        services.AddScoped<ITypeReadRepository, TypeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStationRepository, StationRepository>();

        return services;
    }

    private static IServiceCollection AddContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Database"));
        });

        services.AddDbContext<AppIdentityDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("AuthDatabase"));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }


    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddSingleton<IEveGlobalRateLimit, EveGlobalRateLimit>();

        services.AddHttpClient<IEveApiOpenClientProvider, EveApiClientProvider>(client =>
        {
            client.BaseAddress = new Uri(EveConstants.BaseUrlEsi);
        });

        services.AddHttpClient<IEveApiAuthClientProvider, EveApiClientProvider>(client =>
        {
            client.BaseAddress = new Uri(EveConstants.BaseUrlEsi);
        });

        services.AddSingleton<IRedisProvider, RedisProvider>();

        return services;
    }
}
