using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("type_id");
        builder.Property(p => p.Quantity)
            .HasColumnName("quantity");
        builder.Property(p => p.Time)
            .HasColumnName("time");
        builder.Property(p => p.MaxProductionLimit)
            .HasColumnName("max_production_limit");
        builder.Property(p => p.BlueprintId)
            .HasColumnName("blueprint_id");

        builder.HasOne(p => p.Type)
            .WithOne(t => t.Product)
            .HasForeignKey<ProductEntity>(p => p.Id);
    }
}
