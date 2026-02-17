using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.QRCode).IsRequired().HasColumnType("nvarchar(MAX)");
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasIndex(x => x.QRCode).IsUnique();
    }
}
