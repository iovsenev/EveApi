using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class ReprocessMaterialEntityConfiguration : IEntityTypeConfiguration<ReprocessMaterialEntity>
{
    public void Configure(EntityTypeBuilder<ReprocessMaterialEntity> builder)
    {
        builder.ToTable("reprocess_materials");

        builder.HasKey(pe => new { pe.TypeId, pe.MaterialId });

        builder.Property(pe => pe.TypeId)
            .HasColumnName("type_id");
        builder.Property(pe => pe.MaterialId)
            .HasColumnName("material_id");

        builder.Property(pe => pe.Quantity)
            .HasColumnName("quantity");

        builder.HasOne(pe => pe.Type)
            .WithMany(t => t.ReprocessComponents)
            .HasForeignKey(t => t.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pe => pe.Material)
            .WithMany()
            .HasForeignKey(pe => pe.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
