using Eve.Domain.Entities;
using Eve.Application.StaticDataLoaders.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
public class CategoryFileReader
{
    private readonly IFileReader _reader;
    private readonly ILogger<CategoryFileReader> _logger;

    public CategoryFileReader(
        IFileReader reader, 
        ILogger<CategoryFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<List<CategoryEntity>> GetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for categories");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var categories = new List<CategoryEntity>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;

            var nameDict = entityData["name"] as Dictionary<object, object>;

            var category = new CategoryEntity();

            category.Id = entry.Key;
            category.Name = nameDict != null && nameDict.ContainsKey("en") ? nameDict["en"].ToString() : "Unknown";
            category.Published = Convert.ToBoolean(entityData["published"]);

            categories.Add(category);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{categories.Count} categories formed in {millSec} ms");

        return categories;
    }
}
