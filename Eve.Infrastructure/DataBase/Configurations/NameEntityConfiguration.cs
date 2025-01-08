using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class NameEntityConfiguration : IEntityTypeConfiguration<NameEntity>
{
    public void Configure(EntityTypeBuilder<NameEntity> builder)
    {
        builder.ToTable("names");

        builder.HasKey(name => name.Id);

        builder.Property(name => name.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnName("id");

        builder.Property(name => name.Name)
            .HasColumnName("name");
    }
}
