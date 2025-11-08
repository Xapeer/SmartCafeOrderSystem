namespace Application.Dtos.Order;

public class GetOrderTotalDto
{
    public decimal total { get; set; }
    public decimal totalWithDiscount { get; set; }
}