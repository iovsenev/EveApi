using Eve.Domain.Entities.Universe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class RegionEntityConfiguration : IEntityTypeConfiguration<RegionEntity>
{
    public void Configure(EntityTypeBuilder<RegionEntity> builder)
    {
        builder.ToTable("regions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .HasColumnName("id");
        builder.Property(r => r.NameId)
            .HasColumnName("name_id");

        builder.HasOne(r => r.Name)
            .WithMany()
            .HasForeignKey(r => r.NameId);
    }
}
