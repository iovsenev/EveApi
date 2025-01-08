namespace Eve.Domain.Entities.Universe;
public class SolarSystemEntity
{
    public int Id { get; set; }
    public float SecurityStatus { get; set; }
    public bool IsHub { get; set; }

    public int NameId { get; set; }
    public NameEntity Name { get; set; }

    public int ConstellationId { get; set; }
    public ConstellationEntity Constellation { get; set; }

    public int RegionId { get; set; }
    public RegionEntity Region { get; set; }
}
