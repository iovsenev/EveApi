using Eve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eve.Infrastructure.DataBase.Configurations;
public class IconEntityConfiguration : IEntityTypeConfiguration<IconEntity>
{
    public void Configure(EntityTypeBuilder<IconEntity> builder)
    {
        builder.ToTable("icons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnName("id");
        builder.Property(x => x.FileName)
            .HasColumnName("file_name");
    }
}
