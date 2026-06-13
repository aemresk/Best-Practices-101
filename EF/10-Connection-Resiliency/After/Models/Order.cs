public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
