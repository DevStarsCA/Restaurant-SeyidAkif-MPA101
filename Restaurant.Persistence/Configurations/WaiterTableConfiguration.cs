using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Persistence.Configurations;

public class WaiterTableConfiguration : IEntityTypeConfiguration<WaiterTable>
{
    public void Configure(EntityTypeBuilder<WaiterTable> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Waiter)
            .WithMany(x => x.WaiterTables)
            .HasForeignKey(x => x.WaiterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Table)
            .WithMany(x => x.WaiterTables)
            .HasForeignKey(x => x.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.WaiterId, x.TableId, x.IsActive });
    }
}