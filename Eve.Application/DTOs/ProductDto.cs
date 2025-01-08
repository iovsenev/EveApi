namespace Eve.Application.DTOs;
public class ProductDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int Time { get; set; }
    public int BlueprintId { get; set; }
    public List<MaterialDto> Materials { get; set; }
}
