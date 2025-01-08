using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;
public class NamesFileReader
{
    private readonly FileReader _reader;
    private readonly ILogger<NamesFileReader> _logger;


    public NamesFileReader(
        FileReader reader, 
        ILogger<NamesFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }


    public async Task<List<NameEntity>> GetData(string filePath)
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

        var names = new List<NameEntity>();

        foreach (var entry in rawEntities)
        {
            var typeEntity = new NameEntity();

            typeEntity.Id = Convert.ToInt32(entry["itemID"]);
            typeEntity.Name = entry["itemName"].ToString();

            names.Add(typeEntity);
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{names.Count} names formed in {millSec} ms");

        return names;
    }
}
