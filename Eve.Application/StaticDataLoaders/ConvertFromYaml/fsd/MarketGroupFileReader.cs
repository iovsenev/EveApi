using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
public class MarketGroupFileReader
{
    private readonly FileReader _reader;
    private readonly ILogger<MarketGroupFileReader> _logger;

    public MarketGroupFileReader(
        FileReader reader,
        ILogger<MarketGroupFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }


    public async Task<List<MarketGroupEntity>> GetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for Icons");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var groups = new List<MarketGroupEntity>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;
            var nameDict = entityData["nameID"] as Dictionary<object, object>;
            var descriptionDict = entityData.ContainsKey("descriptionID")
                ? entityData["descriptionID"] as Dictionary<object, object>
                : null;

            var group = new MarketGroupEntity();

            group.Id = entry.Key;
            group.Name = nameDict != null && nameDict.ContainsKey("en") ? nameDict["en"].ToString() : "Unknown";
            group.Description = descriptionDict != null && descriptionDict.ContainsKey("en")
                ? CustomDecoder.HtmlDecode(descriptionDict["en"].ToString())
                : "";
            group.ParentId = entityData.ContainsKey("parentGroupID")
                ? Convert.ToInt32(entityData["parentGroupID"])
                : null;
            group.HasTypes = Convert.ToBoolean(entityData["hasTypes"]);
            group.IconId = entityData.ContainsKey("iconID")
                ? Convert.ToInt32(entityData["iconID"])
                : null;

            groups.Add(group);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{groups.Count} market groups formed in {millSec} ms");

        return groups;
    }
}
