using Eve.Application.AuthServices.AuthTokenService;
using Eve.Application.DTOs;
using Eve.Application.InternalServices;
using Eve.Application.InternalServices.TokenService;
using Eve.Application.Mapping;
using Eve.Application.QueryServices;
using Eve.Application.QueryServices.Stations.GetStations;
using Eve.Application.QueryServices.Types.GetChildTypesForMarketGroup;
using Eve.Application.StaticDataLoaders;
using Eve.Application.StaticDataLoaders.Common;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Universe;
using Eve.Domain.Interfaces.ApiServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Eve.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddServices(services);

        services.AddAutoMapper(typeof(MarketProfile));

        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<EveSsoJwtValidator>();

        services.AddScoped<EntityLoader>();
        services.AddScoped<ILoadOrdersService,LoadOrdersService>();
        services.AddScoped<IEsiTokenService,EsiTokenService>();

        services.AddScoped<TypesFileReader>();
        services.AddScoped<BlueprintsFileReader>();
        services.AddScoped<CategoryFileReader>();
        services.AddScoped<GroupFileReader>();
        services.AddScoped<MarketGroupFileReader>();
        services.AddScoped<ReprocessMaterialsFileReader>();
        services.AddScoped<UniverseFileReader>();
        services.AddScoped<NamesFileReader>();
        services.AddScoped<IconFileReader>();
        services.AddScoped<StationFileReader>();

        services.AddScoped<FileReader>();

        services.AddRequestHandlers(typeof(GetChildTypesForMarketGroupIdHandler));

        services.AddScoped<IService<StationNameDto>, GetStationName>();
        services.AddScoped<ITokenService, TokenService>();
    }

    private static IServiceCollection AddRequestHandlers(this IServiceCollection services, Type assemblyType)
    {
        services.AddScoped<IQueryHandler, QueryHandler>();

        if (services == null) 
            throw new ArgumentNullException(nameof(assemblyType));

        var assembly = assemblyType.Assembly;
        var scanType = typeof (IRequestHandler<,>);

        services.RegisterScanTypeWithImplementations(assembly, scanType);

        return services;
    }

    private static void RegisterScanTypeWithImplementations(
        this IServiceCollection services,
        Assembly assembly,
        Type typeScan)
    {
        var CommandHandlers = ScanTypes(assembly, typeScan);

        foreach (var handler in CommandHandlers)
        {
            var abstraction = handler.GetTypeInfo()
                .ImplementedInterfaces
                .First(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeScan);

            services.AddScoped(abstraction, handler);
        }
    }

    private static IEnumerable<Type> ScanTypes(Assembly assembly, Type typeToScanFor)
    {
        return assembly.GetTypes()
            .Where(type => type is
            {
                IsClass: true,
                IsAbstract: false
            } && type.GetInterfaces()
                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeToScanFor));
    }
}