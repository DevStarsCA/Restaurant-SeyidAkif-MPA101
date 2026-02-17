using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ChatType).HasConversion<int>();
        builder.Property(x => x.SenderName).HasMaxLength(100);

        builder.HasOne(x => x.Table)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Waiter)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.WaiterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}