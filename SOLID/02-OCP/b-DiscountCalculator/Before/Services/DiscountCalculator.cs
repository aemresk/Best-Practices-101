// ❌ OCP İHLALİ: Yeni indirim kuralı = bu metodun içini değiştirmek
// Pazarlama "Siyah Cuma indirimi ekle" dediğinde bu sınıf açılır ve mevcut kurallar bozulabilir
public class DiscountCalculator
{
    public DiscountResult Calculate(CalculateRequest request)
    {
        decimal totalDiscount = 0;
        var appliedRules = new List<string>();

        // ❌ KURAL 1: Premium üye indirimi — değişirse bu blok açılır
        if (request.IsPremiumMember)
        {
            totalDiscount += 0.10m;
            appliedRules.Add("PremiumMember(%10)");
        }

        // ❌ KURAL 2: İlk sipariş indirimi — değişirse bu blok açılır
        if (request.IsFirstOrder)
        {
            totalDiscount += 0.05m;
            appliedRules.Add("FirstOrder(%5)");
        }

        // ❌ KURAL 3: Kupon indirimi — yeni kupon tipi = burayı açmak
        if (request.HasCoupon && request.CouponCode == "SUMMER20")
        {
            totalDiscount += 0.20m;
            appliedRules.Add("Coupon:SUMMER20(%20)");
        }
        else if (request.HasCoupon && request.CouponCode == "WELCOME10")
        {
            totalDiscount += 0.10m;
            appliedRules.Add("Coupon:WELCOME10(%10)");
        }

        // ❌ KURAL 4: Sepet tutarı bazlı indirim — kademeler değişince burayı açmak
        if (request.TotalAmount >= 1000)
        {
            totalDiscount += 0.08m;
            appliedRules.Add("CartAmount>=1000(%8)");
        }
        else if (request.TotalAmount >= 500)
        {
            totalDiscount += 0.03m;
            appliedRules.Add("CartAmount>=500(%3)");
        }

        // ❌ Maksimum indirim sınırı da burada gömülü
        totalDiscount = Math.Min(totalDiscount, 0.40m);

        var finalAmount = request.TotalAmount * (1 - totalDiscount);
        return new DiscountResult(request.TotalAmount, totalDiscount, finalAmount, string.Join(", ", appliedRules));
    }
}
