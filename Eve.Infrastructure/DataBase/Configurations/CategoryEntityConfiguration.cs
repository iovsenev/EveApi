using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("category_id");
        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnName("category_name");
        builder.Property(c => c.Published)
            .IsRequired()
            .HasColumnName("published");
    }
}
