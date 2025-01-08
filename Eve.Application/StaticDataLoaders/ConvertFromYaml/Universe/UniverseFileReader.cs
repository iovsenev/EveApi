using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities.Universe;
using Microsoft.Extensions.Logging;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.Universe;

public class UniverseFileReader
{
    private readonly ConcurrentBag<RegionEntity> _regions = new();
    private readonly ConcurrentBag<ConstellationEntity> _constellations = new();
    private readonly ConcurrentBag<SolarSystemEntity> _systems = new();

    private readonly FileReader _reader;
    private readonly ILogger<UniverseFileReader> _logger;
    private volatile bool _initialized = false;

    public UniverseFileReader(FileReader reader, ILogger<UniverseFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public IReadOnlyCollection<RegionEntity> Regions => EnsureInitialized(_regions);
    public IReadOnlyCollection<ConstellationEntity> Constellations => EnsureInitialized(_constellations);
    public IReadOnlyCollection<SolarSystemEntity> Systems => EnsureInitialized(_systems);

    public async Task ReadData(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
            throw new ArgumentException("Invalid root path", nameof(rootPath));

        _logger.LogInformation("Starting data reading...");
        var stopwatch = Stopwatch.StartNew();

        await ProcessDirectory(rootPath, -1, -1);

        stopwatch.Stop();
        _logger.LogInformation(
            $"Processed {_systems.Count} systems\n, {_constellations.Count} constellations\n, {_regions.Count} regions\n in {stopwatch.ElapsedMilliseconds} ms.");

        _initialized = true;
    }

    private async Task ProcessDirectory(string path, int constellationId, int regionId)
    {
        var directories = Directory.EnumerateDirectories(path).ToList();

        // Параллельная обработка вложенных папок
        var tasks = directories.AsParallel().Select(async directory =>
        {
            if (File.Exists(Path.Combine(directory, "region.yaml")))
                await ProcessRegion(directory);
            else if (File.Exists(Path.Combine(directory, "constellation.yaml")))
                await ProcessConstellation(directory, regionId);
            else if (File.Exists(Path.Combine(directory, "solarsystem.yaml")))
                await ProcessSolarSystem(directory, constellationId, regionId);
            else
                await ProcessDirectory(directory, constellationId, regionId);
        });

        await Task.WhenAll(tasks);
    }

    private async Task ProcessRegion(string directory)
    {
        var rawData = await _reader.ReadYamlFileUniverse(Path.Combine(directory, "region.yaml"));
        var entity = new RegionEntity
        {
            Id = Convert.ToInt32(rawData["regionID"]),
            NameId = Convert.ToInt32(rawData["regionID"])
        };
        _regions.Add(entity);

        await ProcessDirectory(directory, -1, entity.Id);
    }

    private async Task ProcessConstellation(string directory, int regionId)
    {
        var rawData = await _reader.ReadYamlFileUniverse(Path.Combine(directory, "constellation.yaml"));
        var entity = new ConstellationEntity
        {
            Id = Convert.ToInt32(rawData["constellationID"]),
            NameId = Convert.ToInt32(rawData["constellationID"]),
            RegionId = regionId
        };
        _constellations.Add(entity);

        await ProcessDirectory(directory, entity.Id, regionId);
    }

    private async Task ProcessSolarSystem(string directory, int constellationId, int regionId)
    {
        var rawData = await _reader.ReadYamlFileUniverse(Path.Combine(directory, "solarsystem.yaml"));
        var entity = new SolarSystemEntity
        {
            Id = Convert.ToInt32(rawData["solarSystemID"]),
            NameId = Convert.ToInt32(rawData["solarSystemID"]),
            SecurityStatus = float.Parse(rawData["security"].ToString(), CultureInfo.InvariantCulture),
            IsHub = Convert.ToBoolean(rawData["hub"]),
            ConstellationId = constellationId,
            RegionId = regionId
        };
        _systems.Add(entity);
    }

    private IReadOnlyCollection<T> EnsureInitialized<T>(ConcurrentBag<T> collection)
    {
        if (!_initialized)
            throw new InvalidOperationException("Data not initialized.");
        return collection.ToList();
    }
}

