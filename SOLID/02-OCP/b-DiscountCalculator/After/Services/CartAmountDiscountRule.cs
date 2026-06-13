// ✅ Sepet tutarı bazlı indirim izole
public class CartAmountDiscountRule : IDiscountRule
{
    public string RuleName => "CartAmount";
    public bool IsApplicable(CalculateRequest r) => r.TotalAmount >= 500;
    public decimal GetDiscountRate(CalculateRequest r) => r.TotalAmount >= 1000 ? 0.08m : 0.03m;
}
