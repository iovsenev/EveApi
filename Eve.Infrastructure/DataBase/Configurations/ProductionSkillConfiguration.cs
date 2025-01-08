using Eve.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class ProductionSkillConfiguration : IEntityTypeConfiguration<ProductSkillEntity>
{
    public void Configure(EntityTypeBuilder<ProductSkillEntity> builder)
    {
        builder.ToTable("production_skills");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Id)
            .HasColumnName("id");
        builder.Property(ps => ps.Level)
            .HasColumnName("level");
        builder.Property(ps => ps.TypeId)
            .HasColumnName("type_id");
        builder.Property(ps => ps.ProductId)
            .HasColumnName("product_id");

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.Skills)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Type)
            .WithMany()
            .HasForeignKey(p => p.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
