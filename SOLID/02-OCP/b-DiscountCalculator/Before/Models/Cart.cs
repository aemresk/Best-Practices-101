public class Cart
{
    public required string CustomerId { get; set; }
    public bool IsPremiumMember { get; set; }
    public bool IsFirstOrder { get; set; }
    public bool HasCoupon { get; set; }
    public string? CouponCode { get; set; }
    public decimal TotalAmount { get; set; }
}

public record CalculateRequest(
    string CustomerId,
    decimal TotalAmount,
    bool IsPremiumMember,
    bool IsFirstOrder,
    bool HasCoupon,
    string? CouponCode);

public record DiscountResult(decimal OriginalAmount, decimal DiscountRate, decimal FinalAmount, string AppliedRules);
