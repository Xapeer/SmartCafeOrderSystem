using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IdentityUserId { get; set; } = string.Empty;
    
    public IdentityUser IdentityUser { get; set; } = null!;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}