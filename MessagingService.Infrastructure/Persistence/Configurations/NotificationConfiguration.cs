using MessagingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingService.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb") // For PostgreSQL. Use "nvarchar(max)" for SQL Server
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new()
            );
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Critical indexes for performance
        builder.HasIndex(x => new { x.UserId, x.Status, x.CreatedAt })
            .HasDatabaseName("IX_Notifications_User_Status_CreatedAt")
            .IncludeProperties(x => new { x.Title, x.Message, x.Type, x.Priority, x.ReadAt, x.ActionUrl });
        builder.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("IX_Notifications_User_Status");
    }
}