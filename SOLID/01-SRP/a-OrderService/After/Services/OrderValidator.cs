// ✅ Tek sorumluluk: yalnızca sipariş validasyonu
public class OrderValidator
{
    public void Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Müşteri adı zorunlu");
        if (string.IsNullOrWhiteSpace(request.CustomerEmail) || !request.CustomerEmail.Contains('@'))
            throw new ArgumentException("Geçerli bir e-posta adresi giriniz");
        if (request.Amount <= 0)
            throw new ArgumentException("Tutar sıfırdan büyük olmalı");
    }
}
