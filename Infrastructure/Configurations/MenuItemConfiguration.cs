using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        // MenuItem → OrderItem (1:N)
        builder
            .HasMany(mi => mi.OrderItems)
            .WithOne(oi => oi.MenuItem)
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}