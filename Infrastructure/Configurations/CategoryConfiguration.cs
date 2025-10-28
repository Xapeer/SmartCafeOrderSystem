using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Category → MenuItem (1:N)
        builder
            .HasMany(c => c.MenuItems)
            .WithOne(mi => mi.Category)
            .HasForeignKey(mi => mi.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}