namespace Eve.Domain.Entities.Universe;
public class RegionEntity
{
    public int Id { get; set; }
    public int NameId { get; set; }
    public NameEntity Name { get; set; }

    public ICollection<ConstellationEntity> Constellations { get; set; }
}
