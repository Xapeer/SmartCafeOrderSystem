using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        // Discount → Order (1:N)
        builder
            .HasMany(d => d.Orders)
            .WithOne(o => o.Discount)
            .HasForeignKey(o => o.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}