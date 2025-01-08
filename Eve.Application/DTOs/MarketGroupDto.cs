namespace Eve.Application.DTOs;
public class MarketGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public bool HasTypes { get; set; }
    public string? IconFileName { get; set; }
}
