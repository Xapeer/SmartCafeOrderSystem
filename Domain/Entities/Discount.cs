namespace Domain.Entities;

public class Discount
{
    public int Id { get; set; }
    public decimal DiscountPercent { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}