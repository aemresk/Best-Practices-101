// ✅ OCP: Genişleme noktası — her yeni ödeme yöntemi bu interface'i implement eder
// PaymentProcessor hiç değişmez; sadece yeni bir sınıf eklenir
public interface IPaymentStrategy
{
    string Method { get; }          // "CreditCard", "PayPal" vb.
    string Execute(decimal amount); // işlem sonucunu (status) döndürür
}
