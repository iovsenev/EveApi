namespace Eve.Domain.ExternalTypes;

public class TypeMarketHistoryInfo
{
    public double Average { get; set; }
    public string Date { get; set; }
    public double Highest { get; set; }
    public double Lowest { get; set; }
    public long OrderCount { get; set; }
    public long Volume { get; set; }
}
