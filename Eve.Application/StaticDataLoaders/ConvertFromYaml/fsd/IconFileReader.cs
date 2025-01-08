using Eve.Domain.Entities;
using Eve.Application.StaticDataLoaders.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
public class IconFileReader
{
    private readonly FileReader _reader;
    private readonly ILogger<IconFileReader> _logger;

    public IconFileReader(
        FileReader reader, 
        ILogger<IconFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<List<IconEntity>> GetData(string filePath)
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

        var icons = new List<IconEntity>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;

            var icon = new IconEntity();

            icon.Id = entry.Key;
            var fileName = entityData["iconFile"].ToString();
            var parsedName = fileName?.Split(@"/");

            if (!parsedName.Last().Contains(".png") && !parsedName.Last().Contains(".jpg"))
                throw new Exception($"invalid data from file name {parsedName.Last()} with id {entry.Key} with name {fileName}");

            icon.FileName = parsedName.Last();

            icons.Add(icon);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{icons.Count} icons formed in {millSec} ms");

        return icons;
    }
}
