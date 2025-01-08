using Eve.Domain.Entities.Universe;

namespace Eve.Domain.Entities;
public class StationEntity
{
    public long Id {  get; set; }
    public string Name {  get; set; }
    public int? CorporationId { get; set; }
    public float? DockingCostPerVolume { get; set; }
    public int? MaxShipVolumeDockable {  get; set; }
    public int? OfficeRentalCost {  get; set; }
    public int? OperationID {  get; set; }
    public float? ReprocessingEfficiency {  get; set; }
    public int? ReprocessingHangarFlag {  get; set; }
    public float? ReprocessingStationsTake {  get; set; }
    public double? Security {  get; set; }

    public int TypeID {  get; set; }
    public TypeEntity Type { get; set; }

    public int RegionID {  get; set; }
    public RegionEntity Region { get; set; }

    public int ConstellationId { get; set; }
    public ConstellationEntity Constellation { get; set; }

    public int SolarSystemID {  get; set; }
    public SolarSystemEntity SolarSystem { get; set; }
    
}
