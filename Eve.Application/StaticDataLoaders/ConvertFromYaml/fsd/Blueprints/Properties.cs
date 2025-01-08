namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;

internal class Properties
{
    public int Time { get; set; }
    public List<Material> Materials { get; set; } = new();
    public List<Material> Product { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
}
