using Eve.Domain.Entities.Universe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
class SolarSystemEntityConfiguration : IEntityTypeConfiguration<SolarSystemEntity>
{
    public void Configure(EntityTypeBuilder<SolarSystemEntity> builder)
    {
        builder.ToTable("solar_systems");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.NameId)
            .HasColumnName("name_id");
        builder.Property(s=> s .Id)
            .HasColumnName("id");
        builder.Property(s => s.SecurityStatus)
            .HasColumnName("security_status");
        builder.Property(s => s.IsHub)
            .HasColumnName("is_hub");

        builder.HasOne(s => s.Constellation)
            .WithMany(c => c.SolarSystems)
            .HasForeignKey(s => s.ConstellationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Region)
            .WithMany()
            .HasForeignKey(s => s.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss => ss.Name)
            .WithMany()
            .HasForeignKey(ss => ss.NameId);
    }
}
