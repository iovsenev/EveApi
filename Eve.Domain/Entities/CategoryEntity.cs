namespace Eve.Domain.Entities;
public class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Published { get; set; }

    public List<GroupEntity> Groups { get; set; } = new List<GroupEntity>();
}
