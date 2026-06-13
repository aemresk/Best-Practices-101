// ✅ OCP genişleme noktası — her yeni indirim kuralı bu interface'i implement eder
// DiscountCalculator hiç değişmez; sadece yeni bir IDiscountRule sınıfı eklenir
public interface IDiscountRule
{
    string RuleName { get; }
    bool IsApplicable(CalculateRequest request);
    decimal GetDiscountRate(CalculateRequest request);
}
