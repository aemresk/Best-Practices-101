# 04 — Interface Segregation Principle (ISP)

> **"İstemciler, kullanmadıkları metodlara bağımlı olmaya zorlanmamalıdır."**

Şişman (fat) interface'ler, onu implement eden sınıfları ilgisiz metodlarla yükler.

## Senaryolar

| # | Senaryo | Before İhlali | After Çözümü |
|---|---------|--------------|--------------|
| a | OrderRepository | Tek şişman IOrderRepository — okuma/yazma/rapor iç içe | `IOrderReader`, `IOrderWriter`, `IOrderReporter` — ayrı sözleşmeler |
| b | NotificationService | Tek INotificationService — Email+SMS+Push iç içe | `IEmailSender`, `ISmsSender`, `IPushNotifier` — kanal başına interface |

## Tehlike İşaretleri

- `throw new NotImplementedException()` ile dolan metodlar
- Interface'deki metodların yarısını kullanmayan sınıflar
- "Bunu implement etmek zorundayım ama kullanmıyorum" yorumları
