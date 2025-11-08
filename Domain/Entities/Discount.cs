namespace Domain.Entities;

public class Discount
{
    public int Id { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime EndTime { get; set; }
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}