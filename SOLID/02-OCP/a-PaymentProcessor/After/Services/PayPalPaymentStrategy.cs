// ✅ PayPal mantığı izole — diğer stratejilerden bağımsız
public class PayPalPaymentStrategy : IPaymentStrategy
{
    public string Method => "PayPal";

    public string Execute(decimal amount)
    {
        Console.WriteLine($"[PayPal] PayPal hesabına yönlendiriliyor... {amount:C2}");
        Console.WriteLine("[PayPal] PayPal token doğrulandı");
        return "Completed";
    }
}
