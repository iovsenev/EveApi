using Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Universe;
using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.Entities.Universe;
using Eve.Domain.Interfaces.DataBaseAccess.Write;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders;
public class EntityLoader
{
    private readonly CategoryFileReader _categoriesReader;
    private readonly GroupFileReader _groupsReader;
    private readonly MarketGroupFileReader _marketGroupsReader;
    private readonly TypesFileReader _typesReader;
    private readonly ReprocessMaterialsFileReader _materialsReader;
    private readonly BlueprintsFileReader _blueprintsReader;
    private readonly UniverseFileReader _universeReader;
    private readonly IconFileReader _iconReader;
    private readonly NamesFileReader _nameReader;
    private readonly StationFileReader _stationsReader;
    private readonly ILoadRepository _loader;
    private readonly ILogger<EntityLoader> _logger;
    private readonly IConfiguration _config;

    private List<TypeEntity> _types = new();
    private List<CategoryEntity> _categories = new();
    private List<GroupEntity> _groups = new();
    private List<MarketGroupEntity> _marketGroups = new();
    private List<ProductEntity> _products = new();
    private List<ReprocessMaterialEntity> _reprocessMaterials = new();
    private List<SolarSystemEntity> _solarSystems = new();
    private List<ConstellationEntity> _constellations = new();
    private List<RegionEntity> _regions = new();
    private List<IconEntity> _icons = new();
    private List<NameEntity> _names = new();
    private List<StationEntity> _stations = new();

    public EntityLoader(
        CategoryFileReader categoriesReader,
        GroupFileReader groupsReader,
        MarketGroupFileReader marketGroupsReader,
        TypesFileReader typesReader,
        ReprocessMaterialsFileReader materialsReader,
        BlueprintsFileReader blueprintsReader,
        UniverseFileReader universeReader,
        NamesFileReader names,
        IconFileReader iconReader,
        StationFileReader stationsReader,
        ILoadRepository loader,
        ILogger<EntityLoader> logger,
        IConfiguration config
        )
    {
        _categoriesReader = categoriesReader;
        _groupsReader = groupsReader;
        _marketGroupsReader = marketGroupsReader;
        _typesReader = typesReader;
        _materialsReader = materialsReader;
        _blueprintsReader = blueprintsReader;
        _universeReader = universeReader;
        _iconReader = iconReader;
        _nameReader = names;
        _stationsReader = stationsReader;
        _loader = loader;
        _logger = logger;
        _config = config;
    }

    public async Task Run(CancellationToken token)
    {
        var stopWatch = Stopwatch.StartNew();
        _logger.Log(LogLevel.Information, "Start read Files from SDE");

        var fsdPath = _config["FilesPaths:FSD"];
        var bsdPath = _config["FilesPaths:BSD"];
        var universePath = _config["FilesPaths:Universe"];

        _categories = await _categoriesReader.GetData(Path.Combine(fsdPath, "categories.yaml"));
        _groups = await _groupsReader.GetData(Path.Combine(fsdPath, "groups.yaml"));
        _types = await _typesReader.GetData(Path.Combine(fsdPath, "types.yaml"));
        _marketGroups = await _marketGroupsReader.GetData(Path.Combine(fsdPath, "marketGroups.yaml"));
        _reprocessMaterials = await _materialsReader.GetData(Path.Combine(fsdPath, "typeMaterials.yaml"));
        _products = await _blueprintsReader.GetData(Path.Combine(fsdPath, "blueprints.yaml"));
        _icons = await _iconReader.GetData(Path.Combine(fsdPath, "iconIDs.yaml"));
        _names = await _nameReader.GetData(Path.Combine(bsdPath, "invNames.yaml"));
        _stations = await _stationsReader.GetData(Path.Combine(bsdPath, "staStations.yaml"));
        await _universeReader.ReadData(universePath);

        _solarSystems = _universeReader.Systems.ToList();
        _constellations = _universeReader.Constellations.ToList();
        _regions = _universeReader.Regions.ToList();

        stopWatch.Stop();
        var millSec = stopWatch.Elapsed.TotalMilliseconds;
        _logger.LogInformation($"Read data from SDE is success with time : {millSec} ms");
        stopWatch = Stopwatch.StartNew();

        _logger.LogInformation($"start format Entities for database");

        await Task.WhenAll([
            MergeTypes(),
            MergeUniverse()
            ]);

        stopWatch.Stop();
        millSec = stopWatch.Elapsed.TotalMilliseconds;
        _logger.LogInformation($"format entities for database success with time: {millSec} ms ");

        stopWatch = Stopwatch.StartNew();

        _logger.LogInformation("start write in database");
        await LoadDatabase(token);

        stopWatch.Stop();
        millSec = stopWatch.Elapsed.TotalMilliseconds;
        _logger.LogInformation($"Entities write to database with time : {millSec}");
    }

    private async Task LoadDatabase(CancellationToken token)
    {
        await _loader.InitialDatabase(token);
        await _loader.LoadIconsAsync(_icons, token);
        await _loader.LoadRegionsAsync(_regions, token);
        await _loader.LoadNamesAsync(_names, token);
        await _loader.LoadCategoryAsync(_categories, token);
        await _loader.LoadGroupsAsync(_groups, token);
        await _loader.LoadMarketGroupsAsync(_marketGroups, token);
        await _loader.LoadTypesAsync(_types, token);
        await _loader.SaveChangesAsync(token);
        await _loader.LoadStationsAsync(_stations, token);
        await _loader.SaveChangesAsync(token);
    }

    private async Task MergeUniverse()
    {
        foreach (var constellation in _constellations)
        {
            constellation.SolarSystems = _solarSystems.Where(c => c.ConstellationId == constellation.Id).ToList();
        }

        foreach (var region in _regions)
        {
            region.Constellations = _constellations.Where(c => c.RegionId == region.Id).ToList();
        }
    }

    private async Task MergeTypes()
    {
        var materialsDict = _reprocessMaterials.GroupBy(rm => rm.TypeId).ToDictionary(g => g.Key, g => g.ToList());
        var productDict = _products.GroupBy(p => p.Id).ToDictionary(p => p.Key, p => p.ToList());

        await Task.WhenAll(_types.Select(async type =>
        {
            if (materialsDict.TryGetValue(type.Id, out var materialsEnt))
            {
                type.ReprocessComponents = materialsEnt;
            }

            if (productDict.TryGetValue(type.Id, out var product) && type.Published)
            {
                    type.Product = product[0];
                    type.IsProduct = true;
            }
        }));
    }
}
