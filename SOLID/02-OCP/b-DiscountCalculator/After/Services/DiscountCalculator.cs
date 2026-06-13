// ✅ Bu sınıf değişmez — yeni kural = yeni IDiscountRule + DI kaydı
public class DiscountCalculator
{
    private const decimal MaxDiscount = 0.40m;
    private readonly IEnumerable<IDiscountRule> _rules;

    public DiscountCalculator(IEnumerable<IDiscountRule> rules) => _rules = rules;

    public DiscountResult Calculate(CalculateRequest request)
    {
        var applied = _rules
            .Where(r => r.IsApplicable(request))
            .Select(r => (r.RuleName, Rate: r.GetDiscountRate(request)))
            .ToList();

        var totalDiscount = Math.Min(applied.Sum(r => r.Rate), MaxDiscount);
        var finalAmount   = request.TotalAmount * (1 - totalDiscount);
        var ruleNames     = string.Join(", ", applied.Select(r => $"{r.RuleName}(%{r.Rate * 100:0})"));

        return new DiscountResult(request.TotalAmount, totalDiscount, finalAmount, ruleNames);
    }
}
