// ❌ ISP İHLALİ: Tüm bildirim kanalları tek şişman interface'de
// SmsOnlyNotifier yalnızca SMS gönderiyor ama Email + Push metodlarını implement etmek ZORUNDA
public interface INotificationService
{
    void SendEmail(EmailNotification notification);
    void SendSms(SmsNotification notification);
    void SendPush(PushNotification notification);
}

// ✅ Tümünü destekleyen implementasyon — sorun yok
public class FullNotificationService : INotificationService
{
    public void SendEmail(EmailNotification n) =>
        Console.WriteLine($"[EMAIL] → {n.To} | {n.Subject}");
    public void SendSms(SmsNotification n) =>
        Console.WriteLine($"[SMS] → {n.PhoneNumber} | {n.Message}");
    public void SendPush(PushNotification n) =>
        Console.WriteLine($"[PUSH] → {n.DeviceToken} | {n.Title}");
}

// ❌ ISP İHLALİ: Pazarlama SMS servisi yalnızca SMS istiyor
// Ama Email + Push metodlarını implement etmek zorunda
public class MarketingSmsService : INotificationService
{
    public void SendSms(SmsNotification n) =>
        Console.WriteLine($"[MARKETING-SMS] → {n.PhoneNumber} | {n.Message}");

    // ❌ Kullanmıyorum ama interface zorunlu kılıyor
    public void SendEmail(EmailNotification n) => throw new NotImplementedException("Bu servis yalnızca SMS gönderir");
    public void SendPush(PushNotification n)   => throw new NotImplementedException("Bu servis yalnızca SMS gönderir");
}

// ❌ ISP İHLALİ: Mobil push servisi yalnızca Push istiyor
public class MobilePushService : INotificationService
{
    public void SendPush(PushNotification n) =>
        Console.WriteLine($"[PUSH] → {n.DeviceToken} | {n.Title}: {n.Body}");

    // ❌ Kullanmıyorum ama interface zorunlu kılıyor
    public void SendEmail(EmailNotification n) => throw new NotImplementedException("Bu servis yalnızca Push gönderir");
    public void SendSms(SmsNotification n)     => throw new NotImplementedException("Bu servis yalnızca Push gönderir");
}
