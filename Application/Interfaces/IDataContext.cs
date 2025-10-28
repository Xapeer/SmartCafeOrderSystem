using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IDataContext
{
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Table> Tables { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}