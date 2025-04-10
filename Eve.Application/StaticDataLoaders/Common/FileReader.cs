using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.Common;
public class FileReader
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<FileReader> _logger;
    public FileReader(
        IFileSystem fileSystem,
        ILogger<FileReader> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public async Task<Dictionary<int, Dictionary<string, object>>> ReadYamlFileFSD(string path)
    {
        if (!_fileSystem.Exists(path)){
            _logger.LogWarning($"File not exist for Path: {path}");
            throw new FileLoadException($"File on path: {path} does not exist");
        }

        _logger.LogInformation($"Start read file from path ${path}.");

        var stopWatch = new Stopwatch();  
        stopWatch.Start();

        var yamlContent = _fileSystem.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var rawEntities = deserializer.Deserialize<Dictionary<int, Dictionary<string, object>>>(yamlContent);

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"file reading completed for path: {path} and completed in {millSec} ms");
        return rawEntities;
    }

    public async Task<List<Dictionary<string, object>>> ReadYamlFileBSD(string path)
    {
        if (!_fileSystem.Exists(path))
        {
            _logger.LogWarning($"File not exist for Path: {path}");
            throw new FileLoadException($"File on path: {path} does not exist");
        }

        _logger.LogInformation($"Start read file from path ${path}.");

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var yamlContent = _fileSystem.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var rawEntities = deserializer.Deserialize<List<Dictionary<string, object>>>(yamlContent);

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"file reading completed for path: {path} and completed in {millSec} ms");

        return rawEntities;
    }

    public async Task<Dictionary<string, object>> ReadYamlFileUniverse(string path)
    {
        if (!_fileSystem.Exists(path))
        {
            _logger.LogWarning($"File not exist for Path: {path}");
            throw new FileLoadException($"File on path: {path} does not exist");
        }

        var yamlContent = _fileSystem.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var rawEntities = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

        return rawEntities;
    }
}
