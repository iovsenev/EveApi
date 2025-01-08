namespace Eve.Application.DTOs;
public class TypeOrderDto
{
    public long OrderId { get; set; }
    public int TypeId { get; set; }
    public int SystemId { get; set; }
    public string StationName { get; set; }
    public int MinVolume { get; set; }
    public int VolumeRemain { get; set; }
    public int VolumeTotal { get; set; }
    public double Price { get; set; }
    public bool IsBuyOrder { get; set; }
}
