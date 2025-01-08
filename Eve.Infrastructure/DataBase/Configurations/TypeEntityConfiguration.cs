using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class TypeEntityConfiguration : IEntityTypeConfiguration<TypeEntity>
{
    public void Configure(EntityTypeBuilder<TypeEntity> builder)
    {
        builder.ToTable("types");

        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Name);

        builder.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnName("type_id");

        builder.Property(t => t.MarketGroupId)
            .HasColumnName("market_group_id");
        builder.Property(t => t.GroupId)
            .HasColumnName("group_id");
        builder.Property(t => t.IconId)
            .HasColumnName("icon_id");

        builder.Property(t => t.IsProduct)
            .HasColumnName("is_product");
        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnName("type_name");
        builder.Property(t => t.Description)
            .HasColumnName("description");
        builder.Property(t => t.Capacity)
            .HasColumnName("capacity");
        builder.Property(t => t.Mass)
            .HasColumnName("mass");
        builder.Property(t => t.PackagedVolume)
            .HasColumnName("packaged_volume");
        builder.Property(t => t.PortionSize)
            .HasColumnName("portion_size");
        builder.Property(t => t.Radius)
            .HasColumnName("radius");
        builder.Property(t => t.Volume)
            .HasColumnName("volume");
        builder.Property(t => t.Published)
            .IsRequired()
            .HasColumnName("published");


        builder.HasOne(t => t.MarketGroup)
            .WithMany(mg => mg.Types)
            .HasForeignKey(t => t.MarketGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.Group)
            .WithMany(g => g.Types)
            .HasForeignKey(t => t.GroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(g => g.Icon)
            .WithMany()
            .HasForeignKey(g => g.IconId);
    }
}
