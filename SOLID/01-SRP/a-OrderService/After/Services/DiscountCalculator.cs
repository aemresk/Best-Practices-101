// ✅ Tek sorumluluk: yalnızca indirim hesaplama
// İndirim kuralları değişirse sadece bu sınıf açılır
public class DiscountCalculator
{
    public decimal Calculate(decimal amount)
    {
        if (amount >= 5000)      return 0.15m;
        if (amount >= 1000)      return 0.10m;
        if (amount >= 500)       return 0.05m;
        return 0m;
    }
}
