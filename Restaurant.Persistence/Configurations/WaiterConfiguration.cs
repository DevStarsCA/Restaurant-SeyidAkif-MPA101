using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Persistence.Configurations;

public class WaiterConfiguration : IEntityTypeConfiguration<Waiter>
{
    public void Configure(EntityTypeBuilder<Waiter> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        builder.HasOne(x => x.AppUser)
            .WithOne()
            .HasForeignKey<Waiter>(x => x.AppUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}