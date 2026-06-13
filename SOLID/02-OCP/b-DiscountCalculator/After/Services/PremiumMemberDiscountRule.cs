// ✅ Premium üye kuralı izole — sadece bu sınıf bu kuraldan sorumlu
public class PremiumMemberDiscountRule : IDiscountRule
{
    public string RuleName => "PremiumMember";
    public bool IsApplicable(CalculateRequest r) => r.IsPremiumMember;
    public decimal GetDiscountRate(CalculateRequest r) => 0.10m;
}
