# 05 — Dependency Rule

> Bağımlılıklar her zaman içe doğru — dış katmanlar iç katmanlara bağlıdır, tersi olmaz.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Notifications](Before/) | Domain sınıfı `SmtpEmailSender`, `FileAuditLogger` doğrudan kullanıyor |

## Bağımlılık Yönü

```
❌ Before:  OrderService (Domain) → SmtpEmailSender (Infrastructure)
✅ After:   OrderService (Domain) → IEmailSender (Domain Interface)
            SmtpEmailSender (Infrastructure) → IEmailSender (Domain Interface)
```

## Fark

| | Before | After |
|--|--------|-------|
| Domain bağımlılığı | `SmtpEmailSender`, `FileAuditLogger` (concrete) | `IEmailSender`, `IAuditLogger` (interface) |
| SMTP değişince | Domain sınıfı değişmeli | Sadece Infrastructure değişir |
| Test ortamı | Gerçek SMTP çalışır | `NoOpEmailSender` inject edilir |
| Arayüz konumu | Yok | Domain katmanında |

## Kural

```
Arayüzler (interface) Domain'de tanımlanır.
Infrastructure, Domain'i implement eder — Domain, Infrastructure'ı tanımaz.
```

**Tehlike işaretleri:**
- Domain sınıfında `using System.Net.Mail`, `using System.IO`, `using Twilio` gibi namespace'ler
- Constructor parametresi olarak concrete class alınıyor (interface yerine)
- Domain assembly'si Infrastructure assembly'sine proje referansı veriyor
