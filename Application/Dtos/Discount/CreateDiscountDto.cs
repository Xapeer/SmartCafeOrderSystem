namespace Application.Dtos.Discount;

public class CreateDiscountDto
{
    public decimal DiscountPercent { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}