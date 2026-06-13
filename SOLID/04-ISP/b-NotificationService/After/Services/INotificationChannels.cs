// ✅ ISP: Her bildirim kanalı için ayrı interface
// Bir sınıf yalnızca desteklediği kanalı implement eder

public interface IEmailSender
{
    void SendEmail(EmailNotification notification);
}

public interface ISmsSender
{
    void SendSms(SmsNotification notification);
}

public interface IPushNotifier
{
    void SendPush(PushNotification notification);
}

// ✅ Tümünü destekleyen servis — gerektiğinde tüm interface'leri implement eder
public class FullNotificationService : IEmailSender, ISmsSender, IPushNotifier
{
    public void SendEmail(EmailNotification n) =>
        Console.WriteLine($"[EMAIL] → {n.To} | {n.Subject}");
    public void SendSms(SmsNotification n) =>
        Console.WriteLine($"[SMS] → {n.PhoneNumber} | {n.Message}");
    public void SendPush(PushNotification n) =>
        Console.WriteLine($"[PUSH] → {n.DeviceToken} | {n.Title}");
}

// ✅ ISP uyumlu: Yalnızca ISmsSender — Email + Push yoktur, fırlatma yoktur
public class MarketingSmsService : ISmsSender
{
    public void SendSms(SmsNotification n) =>
        Console.WriteLine($"[MARKETING-SMS] → {n.PhoneNumber} | {n.Message}");
}

// ✅ ISP uyumlu: Yalnızca IPushNotifier — diğer metodlar yoktur
public class MobilePushService : IPushNotifier
{
    public void SendPush(PushNotification n) =>
        Console.WriteLine($"[PUSH] → {n.DeviceToken} | {n.Title}: {n.Body}");
}
