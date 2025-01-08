using System.Diagnostics;
using System.Globalization;
using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
public class TypesFileReader
{
    private readonly FileReader _reader;
    private readonly ILogger<TypesFileReader> _logger;

    public TypesFileReader(
        FileReader reader, 
        ILogger<TypesFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<List<TypeEntity>> GetData(string filePath)
    {

        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for types");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var types = new List<TypeEntity>();

        foreach (var entry in rawEntities)
        {
            var type = await CreateType(entry);
            types.Add(type);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{types.Count} types formed in {millSec} ms");

        return types;
    }

    private async Task<TypeEntity> CreateType(KeyValuePair<int, Dictionary<string, object>> entry)
    {
        var entityData = entry.Value;


        var nameDict = entityData["name"] as Dictionary<object, object>;
        var descriptionDict = entityData.ContainsKey("description") ? entityData["description"] as Dictionary<object, object> : null;

        var typeEntity = new TypeEntity();

        typeEntity.Id = entry.Key;
        typeEntity.Name = nameDict != null && nameDict.ContainsKey("en")
            ? nameDict["en"].ToString()
            : "Unknown";
        typeEntity.Description = descriptionDict != null && descriptionDict.ContainsKey("en")
            ? CustomDecoder.HtmlDecode(descriptionDict["en"].ToString())
            : "";
        typeEntity.GroupId = Convert.ToInt32(entityData["groupID"]);
        typeEntity.Published = Convert.ToBoolean(entityData["published"]);

        typeEntity.Mass = entityData.ContainsKey("mass")
            ? float.Parse(entityData["mass"].ToString(), CultureInfo.InvariantCulture)
            : null;
        typeEntity.Volume = entityData.ContainsKey("volume")
            ? float.Parse(entityData["volume"].ToString(), CultureInfo.InvariantCulture)
            : null;
        typeEntity.Radius = entityData.ContainsKey("radius")
            ? float.Parse(entityData["radius"].ToString(), CultureInfo.InvariantCulture)
            : null;
        typeEntity.PortionSize = entityData.ContainsKey("portionSize")
            ? Convert.ToInt32(entityData["portionSize"])
            : null;
        typeEntity.Capacity = entityData.ContainsKey("capacity")
            ? float.Parse(entityData["capacity"].ToString(), CultureInfo.InvariantCulture)
            : null;
        typeEntity.PackagedVolume = entityData.ContainsKey("packagedVolume")
            ? Convert.ToSingle(entityData["packagedVolume"])
            : null;

        typeEntity.IconId = entityData.ContainsKey("iconID")
            ? Convert.ToInt32(entityData["iconID"])
            : null;
        typeEntity.MarketGroupId = entityData.ContainsKey("marketGroupID")
            ? Convert.ToInt32(entityData["marketGroupID"])
            : null;
        return typeEntity;
    }
}
