using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class StationEntityConfiguration : IEntityTypeConfiguration<StationEntity>
{
    public void Configure(EntityTypeBuilder<StationEntity> builder)
    {
        builder.ToTable("stations");

        builder.HasKey(t => t.Id);

        builder.Property(station => station.Id)
            .IsRequired()
            .HasColumnName("id");
        builder.Property(station => station.Name)
            .IsRequired() 
            .HasColumnName("name");
        builder.Property(station => station.CorporationId)
            .HasColumnName("corporation_id");
        builder.Property(station => station.DockingCostPerVolume)
            .HasColumnName("docking_cost_per_volume");
        builder.Property(station => station.MaxShipVolumeDockable)
            .HasColumnName("max_ship_volume_dockable");
        builder.Property(station => station.OfficeRentalCost)
            .HasColumnName("office_rental_cost");
        builder.Property(station => station.OperationID)
            .HasColumnName("operation_id");
        builder.Property(station => station.ReprocessingEfficiency)
            .HasColumnName("reprocessing_efficiency");
        builder.Property(station => station.ReprocessingHangarFlag)
            .HasColumnName("reprocessing_hangar_flag");
        builder.Property(station => station.ReprocessingStationsTake)
            .HasColumnName("reprocessing_station_take");
        builder.Property(station => station.Security)
            .HasColumnName("security");
        builder.Property(station => station.RegionID)
            .HasColumnName("region_id");
        builder.Property(station => station.ConstellationId)
            .HasColumnName("constellation_id");
        builder.Property(station => station.SolarSystemID)
            .HasColumnName("solar_system_id");
        builder.Property(station => station.TypeID)
            .HasColumnName("type_id");

        builder.HasOne(station => station.Region)
            .WithMany()
            .HasForeignKey(station => station.RegionID);

        builder.HasOne(station => station.Constellation)
            .WithMany()
            .HasForeignKey(station => station.ConstellationId);

        builder.HasOne(station => station.SolarSystem)
            .WithMany()
            .HasForeignKey(station => station.SolarSystemID);

        builder.HasOne(station => station.Type)
            .WithMany()
            .HasForeignKey(station => station.TypeID);

    }
}
