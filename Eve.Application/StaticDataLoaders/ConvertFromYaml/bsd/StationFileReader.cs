using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;

public class StationFileReader
{
    private readonly FileReader _reader;
    private readonly ILogger<StationFileReader> _logger;


    public StationFileReader(
        FileReader reader,
        ILogger<StationFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }


    public async Task<List<StationEntity>> GetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for names");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileBSD(filePath);

        var names = new List<StationEntity>();

        foreach (var entry in rawEntities)
        {
            var typeEntity = new StationEntity();

            typeEntity.Id = Convert.ToInt32(entry["stationID"]);
            typeEntity.Name = entry["stationName"].ToString();
            typeEntity.CorporationId = entry.ContainsKey("corporationID")
                ? Convert.ToInt32(entry["stationID"])
                : null;
            typeEntity.DockingCostPerVolume = entry.ContainsKey("dockingCostPerVolume")
                ? float.Parse(entry["dockingCostPerVolume"].ToString(), CultureInfo.InvariantCulture)
                : null;
            typeEntity.MaxShipVolumeDockable = entry.ContainsKey("maxShipVolumeDockable")
                ? Convert.ToInt32(entry["maxShipVolumeDockable"])
                : null;
            typeEntity.OfficeRentalCost = entry.ContainsKey("officeRentalCost")
                ? Convert.ToInt32(entry["officeRentalCost"])
                : null;
            typeEntity.OperationID = entry.ContainsKey("operationID")
                ? Convert.ToInt32(entry["operationID"])
                : null;
            typeEntity.ReprocessingEfficiency = entry.ContainsKey("reprocessingEfficiency")
                ? float.Parse(entry["reprocessingEfficiency"].ToString(), CultureInfo.InvariantCulture)
                : null;
            typeEntity.ReprocessingHangarFlag = entry.ContainsKey("reprocessingHangarFlag")
                ? Convert.ToInt32(entry["reprocessingHangarFlag"])
                : null;
            typeEntity.ReprocessingStationsTake = entry.ContainsKey("reprocessingStationsTake")
                ? float.Parse(entry["reprocessingStationsTake"].ToString(), CultureInfo.InvariantCulture)
                : null;
            typeEntity.Security = entry.ContainsKey("security")
                ? double.Parse(entry["security"].ToString(), CultureInfo.InvariantCulture)
                : null;
            typeEntity.RegionID = Convert.ToInt32(entry["regionID"]);
            typeEntity.ConstellationId = Convert.ToInt32(entry["constellationID"]);
            typeEntity.SolarSystemID = Convert.ToInt32(entry["solarSystemID"]);
            typeEntity.TypeID = Convert.ToInt32(entry["stationTypeID"]);

            names.Add(typeEntity);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{names.Count} names formed in {millSec} ms");

        return names;
    }
}
