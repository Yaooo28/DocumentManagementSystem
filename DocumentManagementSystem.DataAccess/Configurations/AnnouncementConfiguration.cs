using DocumentManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Content).IsRequired();
        builder.Property(a => a.DateCreated).HasDefaultValueSql("GETDATE()"); // SQL Server syntax for default value
    }
}
