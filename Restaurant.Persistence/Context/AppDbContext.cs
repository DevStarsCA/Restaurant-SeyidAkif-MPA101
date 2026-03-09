using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using System.Reflection;

namespace Restaurant.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Table> Tables { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Waiter> Waiters { get; set; }
    public DbSet<WaiterTable> WaiterTables { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<AppUser>(b =>
        {
            b.Property(x => x.RefreshTokenExpiryTime)
                .HasColumnType("timestamp with time zone");
        });

        builder.Entity<Table>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<BasketItem>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<OrderItem>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Waiter>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<WaiterTable>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ChatMessage>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Reservation>().HasQueryFilter(x => !x.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }
}