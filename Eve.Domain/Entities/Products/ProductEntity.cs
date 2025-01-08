namespace Eve.Domain.Entities.Products;
public class ProductEntity
{
    public int Id { get; set; }
    public TypeEntity Type { get; set; }

    public int Quantity { get; set; }
    public int Time { get; set; }
    public int MaxProductionLimit { get; set; }

    public int BlueprintId { get; set; }


    public List<ProductSkillEntity> Skills { get; set; }
    public List<ProductMaterialEntity> Materials { get; set; }
}
