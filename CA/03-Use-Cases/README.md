# 03 — Use Cases (Application Layer)

> Her iş akışı, bağımlılıkları belli, test edilebilir bir Use Case sınıfına taşınmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [User Registration](Before/) | Validasyon + şifreleme + kayıt + bildirim tek metodda |

## Fark

| | Before | After |
|--|--------|-------|
| Validasyon | Her endpoint tekrar yazar | `RegisterUserHandler.Validate` — tek yer |
| Şifreleme | `SHA256.HashData` inline | `IPasswordHasher` arayüzü — algoritma değiştirilebilir |
| E-posta | `Console.WriteLine` inline | `IEmailService.SendWelcome` — gerçek SMTP takılabilir |
| Test | Tüm method mock'lanamaz | Handler bağımlılıkları mock'lanabilir |

## Kural

```
Her use case = bir handler sınıfı.
Handler: koordinatör — iş kuralını bilir, infrastructure'ı bilmez.
```

**Tehlike işaretleri:**
- Bir metodun içinde 5+ farklı sorumluluk (validate, hash, save, email, log)
- `static` yardımcı metodlar iş mantığını içeriyor
- Unit test yazmak için `File.WriteAllText` veya SMTP stub kurmak gerekiyor
