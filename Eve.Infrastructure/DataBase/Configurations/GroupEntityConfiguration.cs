using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class GroupEntityConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable("groups");

        builder.HasKey(g => g.Id);
        builder.HasIndex(g => g.Name);

        builder.Property(g => g.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(g => g.Name)
            .IsRequired()
            .HasColumnName("group_name");
        builder.Property(g => g.CategoryId)
            .HasColumnName("category_id");
        builder.Property(g => g.Published)
            .IsRequired()
            .HasColumnName("published");

        builder.HasOne(g => g.Category)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
