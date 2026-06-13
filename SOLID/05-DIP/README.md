# 05 — Dependency Inversion Principle (DIP)

> **"Yüksek seviyeli modüller, düşük seviyeli modüllere bağlı olmamalıdır. İkisi de soyutlamalara bağlı olmalıdır."**

`new SmtpEmailSender()` yazmak yerine `IEmailSender` enjekte edin.

## Senaryolar

| # | Senaryo | Before İhlali | After Çözümü |
|---|---------|--------------|--------------|
| a | OrderService | `new SmtpEmailSender()` + `new FileLogger()` doğrudan yaratma | `IEmailSender`, `ILogger` inject — test, swap, mock kolaylaşır |
| b | ReportService | `new ExcelExporter()` + `new DatabaseLoader()` doğrudan yaratma | `IReportExporter`, `IDataLoader` inject — format/kaynak değiştirilebilir |

## Tehlike İşaretleri

- `new ConcreteClass()` içinde somut sınıf yaratma
- `private readonly SmtpEmailSender _emailSender = new()` gibi alanlar
- Testi imkânsız kılan sabit bağımlılıklar
