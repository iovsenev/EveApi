using Eve.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;

public class ProductionMaterialConfiguration : IEntityTypeConfiguration<ProductMaterialEntity>
{
    public void Configure(EntityTypeBuilder<ProductMaterialEntity> builder)
    {
        builder.ToTable("production_materials");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Id)
            .HasColumnName("id");
        builder.Property(ps => ps.Quantity)
            .HasColumnName("quantity");
        builder.Property(ps => ps.TypeId)
            .HasColumnName("type_id");
        builder.Property(ps => ps.ProductId)
            .HasColumnName("product_id");

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.Materials)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Type)
            .WithMany()
            .HasForeignKey(p => p.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
