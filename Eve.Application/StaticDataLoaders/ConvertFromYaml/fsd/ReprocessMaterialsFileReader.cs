using Eve.Application.StaticDataLoaders.Common;
using Eve.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd;
public class ReprocessMaterialsFileReader
{
    private readonly IFileReader _reader;
    private readonly ILogger<ReprocessMaterialEntity> _logger;

    public ReprocessMaterialsFileReader(
        IFileReader reader, 
        ILogger<ReprocessMaterialEntity> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<List<ReprocessMaterialEntity>> GetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for materials");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var materials = new List<ReprocessMaterialEntity>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;
            var nameDict = entityData["materials"] as List<object>;


            foreach (var name in nameDict)
            {
                var matDict = name as Dictionary<object, object>;

                var material = new ReprocessMaterialEntity();

                material.TypeId = entry.Key;

                int materialId = matDict.ContainsKey("materialTypeID")
                    ? Convert.ToInt32(matDict["materialTypeID"])
                    : -1;

                int quantity = matDict.ContainsKey("quantity")
                    ? Convert.ToInt32(matDict["quantity"])
                    : -1;

                if (materialId == -1 || quantity == -1)
                    throw new NullReferenceException();

                material.MaterialId = materialId;
                material.Quantity = quantity;

                materials.Add(material);
            }
        }

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{materials.Count} materials formed in {millSec} ms");

        return materials;
    }
}
