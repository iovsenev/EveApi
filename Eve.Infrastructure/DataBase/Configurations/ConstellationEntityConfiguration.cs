using Eve.Domain.Entities.Universe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class ConstellationEntityConfiguration : IEntityTypeConfiguration<ConstellationEntity>
{
    public void Configure(EntityTypeBuilder<ConstellationEntity> builder)
    {
        builder.ToTable("constellations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .HasColumnName("id");
        builder.Property(c => c.NameId)
            .HasColumnName ("name_id");

        builder.HasOne(c => c.Region)
            .WithMany(r => r.Constellations)
            .HasForeignKey(c => c.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Name)
            .WithMany()
            .HasForeignKey(c => c.NameId);
    }
}
