using Eve.Domain.Entities;
using Eve.Application.StaticDataLoaders.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;

public class GroupFileReader
{
    private readonly IFileReader _reader;
    private readonly ILogger<GroupFileReader> _logger;

    public GroupFileReader(
        IFileReader reader,
        ILogger<GroupFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }
    public async Task<List<GroupEntity>> GetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for Groups");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var groups = new List<GroupEntity>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;

            var nameDict = entityData["name"] as Dictionary<object, object>;

            var group = new GroupEntity();

            group.Id = entry.Key;
            group.Name = nameDict != null && nameDict.ContainsKey("en") ? nameDict["en"].ToString() : "Unknown";
            group.CategoryId = Convert.ToInt32(entityData["categoryID"]);
            group.Published = Convert.ToBoolean(entityData["published"]);

            groups.Add(group);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{groups.Count} groups formed in {millSec} ms");

        return groups;
    }
}
