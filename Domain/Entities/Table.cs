namespace Domain.Entities;

public class Table
{
    public int Id { get; set; }
    public bool IsFree { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int NumberOfSeats { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}