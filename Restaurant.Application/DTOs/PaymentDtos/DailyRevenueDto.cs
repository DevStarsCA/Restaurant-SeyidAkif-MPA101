namespace Restaurant.Application.DTOs.PaymentDtos;

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int CashPayments { get; set; }
    public int OnlinePayments { get; set; }
}







