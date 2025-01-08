namespace Eve.Application.DTOs;

public record MaterialDto
{
    public int TypeId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
}