namespace Eve.Domain.Entities.Products;

public class ProductSkillEntity
{
    public int Id { get; set; }
    public int Level { get; set; }

    public int TypeId { get; set; }
    public TypeEntity Type { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }
}
