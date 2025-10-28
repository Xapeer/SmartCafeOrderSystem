using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder
            .HasOne(e => e.IdentityUser)
            .WithOne()
            .HasForeignKey<Employee>(e => e.IdentityUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(e => e.Orders)
            .WithOne(o => o.Waiter)
            .HasForeignKey(o => o.WaiterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}