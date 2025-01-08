namespace Eve.Domain.Entities.Universe;
public class ConstellationEntity
{
    public int Id { get; set; }
    public int NameId { get; set; }
    public NameEntity Name { get; set; }

    public int RegionId { get; set; }
    public RegionEntity Region { get; set; }

    public ICollection<SolarSystemEntity> SolarSystems { get; set; }
}
