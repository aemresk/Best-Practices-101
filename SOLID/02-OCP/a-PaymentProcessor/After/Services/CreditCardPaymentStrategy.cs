// ✅ Kredi kartı mantığı izole — sadece bu sınıf kredi kartından sorumlu
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    public string Method => "CreditCard";

    public string Execute(decimal amount)
    {
        Console.WriteLine($"[CC] Kart doğrulanıyor... {amount:C2} çekiliyor");
        Console.WriteLine("[CC] 3D Secure kontrolü yapıldı");
        return "Approved";
    }
}
