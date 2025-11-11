namespace Application.Dtos.Report;

public class WaiterKpiDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
}