namespace Domain.Entities;

public class Table
{
    public int Id { get; set; }
    public bool IsFree { get; set; } = true;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}