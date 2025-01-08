namespace Eve.Application.DTOs;

public class TypeInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = "";
    public float? Capacity { get; set; }
    public float? PackagedVolume { get; set; }
    public float? Volume { get; set; }
    public bool IsProduct { get; set; } = false;
    public int? MarketGroupId { get; set; }
    public string? IconFileName { get; set; }


    public List<MaterialDto> ReprocessComponents { get; set; } = [];

}
