// ✅ Kripto ödeme eklendi — PaymentProcessor'a tek bir satır bile dokunulmadı
public class CryptoPaymentStrategy : IPaymentStrategy
{
    public string Method => "Crypto";

    public string Execute(decimal amount)
    {
        Console.WriteLine($"[Crypto] Cüzdan adresi doğrulanıyor... {amount:C2} değerinde işlem başlatıldı");
        Console.WriteLine("[Crypto] Blockchain onayı bekleniyor (1-6 blok)");
        return "Broadcasting";
    }
}
