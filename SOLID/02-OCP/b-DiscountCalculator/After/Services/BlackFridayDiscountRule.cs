// ✅ Yeni kural eklendi — mevcut hiçbir sınıfa dokunulmadı
public class BlackFridayDiscountRule : IDiscountRule
{
    private static readonly bool _isBlackFriday =
        DateTime.UtcNow.Month == 11 && DateTime.UtcNow.Day == 29;

    public string RuleName => "BlackFriday";
    public bool IsApplicable(CalculateRequest r) => _isBlackFriday;
    public decimal GetDiscountRate(CalculateRequest r) => 0.25m;
}
