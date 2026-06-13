public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Created";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record OrderSummary(int TotalOrders, decimal TotalRevenue, decimal AverageOrderValue);
