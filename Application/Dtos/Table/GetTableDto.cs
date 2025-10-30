namespace Application.Dtos.Table;

public class GetTableDto
{
    public int Id { get; set; }
    public bool IsFree { get; set; }
    public bool IsActive { get; set; }
    public int NumberOfSeats { get; set; }
}