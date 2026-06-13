public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public required string Method { get; set; }  // "CreditCard" | "PayPal" | "BankTransfer"
    public required string Status { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public record ProcessPaymentRequest(decimal Amount, string Method);
