using Eve.Application.StaticDataLoaders.Common;
using System.Globalization;
using Eve.Domain.Entities.Products;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
public class BlueprintsFileReader
{
    private readonly IFileReader _reader;
    private readonly ILogger<BlueprintsFileReader> _logger;
    public BlueprintsFileReader(
        IFileReader reader, 
        ILogger<BlueprintsFileReader> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<List<ProductEntity>> GetData(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            _logger.LogWarning($"Path to file can not be null or empty.");
            throw new ArgumentNullException(nameof(filePath), "Path to file can not be null or empty.");
        }

        _logger.LogInformation($"Start reading file and generating data for blueprint");
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var rawEntities = await _reader.ReadYamlFileFSD(filePath);

        var blueprints = new List<BlueprintDto>();

        foreach (var entry in rawEntities)
        {
            var entityData = entry.Value;

            var activities = entityData["activities"] as Dictionary<object, object>;

            var copying = activities.ContainsKey("copying")
                ? activities["copying"] as Dictionary<object, object>
                : null;
            var manufacturing = activities.ContainsKey("manufacturing")
                ? activities["manufacturing"] as Dictionary<object, object>
                : null;
            var invention = activities.ContainsKey("invention")
                ? activities["invention"] as Dictionary<object, object>
                : null;
            var researchMaterial = activities.ContainsKey("research_material")
                ? activities["research_material"] as Dictionary<object, object>
                : null;
            var researcTime = activities.ContainsKey("research_time")
                ? activities["research_time"] as Dictionary<object, object>
                : null;

            var typeEntity = new BlueprintDto();

            typeEntity.BlueprintTypeId = Convert.ToInt32(entityData["blueprintTypeID"]);
            typeEntity.MaxProductionLimit = Convert.ToInt32(entityData["maxProductionLimit"]);

            var copyingProp = copying is not null
                ? ReadProperties(copying)
                : null;
            var manufacturingProp = manufacturing is not null
                ? ReadProperties(manufacturing)
                : null;
            var inventionProp = invention is not null
                ? ReadProperties(invention)
                : null;
            var ResearchMaterialsProp = researchMaterial is not null
                ? ReadProperties(researchMaterial)
                : null;
            var ResearchTimeProp = researcTime is not null
                ? ReadProperties(researcTime)
                : null;

            typeEntity.Copying = CreateActivity(copyingProp);
            typeEntity.Manufacturing = CreateActivity(manufacturingProp);
            typeEntity.Invention = CreateActivity(inventionProp);
            typeEntity.ResearchMaterials = CreateActivity(ResearchMaterialsProp);
            typeEntity.ResearchTime = CreateActivity(ResearchTimeProp);

            blueprints.Add(typeEntity);
        }

        var entities = MappingDtoToEntity.GetProductEntity(blueprints);

        stopWatch.Stop();
        var millSec = stopWatch.ElapsedMilliseconds;

        _logger.LogInformation($"{blueprints.Count} pieces of blueprints were generated in {millSec} ms");

        return entities;
    }

    private Properties ReadProperties(Dictionary<object, object> propDict)
    {
        var properties = new Properties();

        var materials = propDict.ContainsKey("materials") ? propDict["materials"] as List<object> : null;
        if (materials != null)
        {
            properties.Materials = AddMaterials(materials);
        }

        var skills = propDict.ContainsKey("skills") ? propDict["skills"] as List<object> : null;
        if (skills != null)
        {
            properties.Skills = AddSkills(skills);
        }

        var products = propDict.ContainsKey("products") ? propDict["products"] as List<object> : null;
        if (products != null)
        {
            properties.Product = AddMaterials(products);
        }

        properties.Time = Convert.ToInt32(propDict["time"]);

        return properties;
    }

    private List<Material> AddMaterials(List<object> copyingMaterials)
    {
        var materials = new List<Material>();

        foreach (var unit in copyingMaterials)
        {
            var unitDict = unit as Dictionary<object, object>;
            var materialEntity = new Material();
            materialEntity.Quantity = Convert.ToInt32(unitDict["quantity"]);
            materialEntity.TypeId = Convert.ToInt32(unitDict["typeID"]);
            materialEntity.Probability = unitDict.ContainsKey("probability")
                ? float.Parse(unitDict["probability"].ToString(), CultureInfo.InvariantCulture)
                : null;
            materials.Add(materialEntity);
        }
        return materials;
    }

    private List<Skill> AddSkills(List<object> copyingSkills)
    {
        var skillList = new List<Skill>();
        foreach (var unit in copyingSkills)
        {
            var unitDict = unit as Dictionary<object, object>;
            var materialEntity = new Skill();
            materialEntity.Level = Convert.ToInt32(unitDict["level"]);
            materialEntity.TypeId = Convert.ToInt32(unitDict["typeID"]);
            skillList.Add(materialEntity);
        }
        return skillList;
    }

    private Activity? CreateActivity(Properties? properties)
    {
        if (properties == null) return null;

        var activity = new Activity
        {
            Materials = properties.Materials,
            Time = properties.Time,
            Skills = properties.Skills,
            Products = properties.Product
        };
        return activity;
    }
}
