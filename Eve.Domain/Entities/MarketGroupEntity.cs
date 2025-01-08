namespace Eve.Domain.Entities;
public class MarketGroupEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool HasTypes { get; set; }

    public int? IconId {  get; set; }
    public IconEntity Icon {  get; set; }
    
    public int? ParentId { get; set; }
    public MarketGroupEntity? ParentGroup { get; set; }

    public List<MarketGroupEntity> ChildGroups { get; set; } = new List<MarketGroupEntity>();

    public List<TypeEntity> Types { get; set; } = new List<TypeEntity>();
}
