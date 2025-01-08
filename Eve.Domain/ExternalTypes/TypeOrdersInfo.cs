namespace Eve.Domain.ExternalTypes;

public class TypeOrdersInfo
{
    public int Duration { get; set; }
    public bool IsBuyOrder { get; set; }
    public string Issued { get; set; }
    public long LocationId { get; set; }
    public int MinVolume { get; set; }
    public long OrderId { get; set; }
    public double Price { get; set; }
    public string Range { get; set; }
    public int SystemId { get; set; }
    public int TypeId { get; set; }
    public int VolumeRemain { get; set; }
    public int VolumeTotal { get; set; }
}