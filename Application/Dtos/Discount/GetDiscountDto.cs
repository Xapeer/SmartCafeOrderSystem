namespace Application.Dtos.Discount;

public class GetDiscountDto
{
    public int Id { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}