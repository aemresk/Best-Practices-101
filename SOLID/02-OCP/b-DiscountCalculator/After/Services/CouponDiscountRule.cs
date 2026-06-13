// ✅ Kupon kuralı izole — kupon kodları değişince sadece bu sınıf açılır
public class CouponDiscountRule : IDiscountRule
{
    private static readonly Dictionary<string, decimal> _coupons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SUMMER20"]  = 0.20m,
        ["WELCOME10"] = 0.10m
    };

    public string RuleName => "Coupon";
    public bool IsApplicable(CalculateRequest r) =>
        r.HasCoupon && r.CouponCode is not null && _coupons.ContainsKey(r.CouponCode);
    public decimal GetDiscountRate(CalculateRequest r) =>
        r.CouponCode is not null && _coupons.TryGetValue(r.CouponCode, out var rate) ? rate : 0m;
}
