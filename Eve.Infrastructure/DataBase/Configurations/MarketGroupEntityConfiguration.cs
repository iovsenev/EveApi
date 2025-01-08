using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class MarketGroupEntityConfiguration : IEntityTypeConfiguration<MarketGroupEntity>
{
    public void Configure(EntityTypeBuilder<MarketGroupEntity> builder)
    {
        builder.ToTable("market_groups");

        builder.HasKey(g => g.Id);
        builder.HasIndex(g => g.Name);

        builder.Property(g => g.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(g => g.ParentId)
            .HasColumnName("parent_group_id");

        builder.Property(g => g.Name)
            .IsRequired()
            .HasColumnName("group_name");
        builder.Property(g => g.Description)
            .HasColumnName("description");
        builder.Property(g => g.IconId)
            .HasColumnName("icon_id");

        builder.HasMany(g => g.ChildGroups)
            .WithOne(g => g.ParentGroup)
            .HasForeignKey(g => g.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(g => g.Icon)
            .WithMany()
            .HasForeignKey(g => g.IconId);
    }
}
