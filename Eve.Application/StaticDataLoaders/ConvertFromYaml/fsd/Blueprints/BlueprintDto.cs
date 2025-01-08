using System.Text.Json;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
public class BlueprintDto
{
    public int BlueprintTypeId { get; set; }
    public int MaxProductionLimit { get; set; }

    public Activity? Copying { get; set; }
    public Activity? Manufacturing { get; set; }
    public Activity? Invention { get; set; }
    public Activity? ResearchMaterials { get; set; }
    public Activity? ResearchTime { get; set; }

    public override string ToString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(this, options);
    }
}

public class Activity
{
    public int Time { get; set; }
    public List<Material> Materials { get; set; } = new();
    public List<Material>? Products { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
}
public class Skill
{
    public int TypeId { get; set; }
    public int Level { get; set; }
}

public class Material
{
    public int TypeId { get; set; }
    public int Quantity { get; set; }
    public float? Probability { get; set; }
}