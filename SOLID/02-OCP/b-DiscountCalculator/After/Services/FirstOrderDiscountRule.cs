// ✅ İlk sipariş kuralı izole
public class FirstOrderDiscountRule : IDiscountRule
{
    public string RuleName => "FirstOrder";
    public bool IsApplicable(CalculateRequest r) => r.IsFirstOrder;
    public decimal GetDiscountRate(CalculateRequest r) => 0.05m;
}
