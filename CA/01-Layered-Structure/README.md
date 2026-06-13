# 01 — Layered Structure (Katmanlı Mimari)

> Kodun, değişim sebebine göre ayrı katmanlara bölünmesi gerekir.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [E-Commerce Order](Before/) | Domain, iş mantığı, veri erişimi ve bildirim tek dosyada |

## Katmanlar

```
After/
├── ECommerce.Domain/          ← Hiçbir şeye bağlı değil (entity, interface)
├── ECommerce.Application/     ← Sadece Domain'e bağlı (use case, handler)
├── ECommerce.Infrastructure/  ← Domain'e bağlı, arayüzleri uygular
└── ECommerce.ConsoleApp/      ← Composition Root (Application + Infrastructure)
```

**Bağımlılık yönü:** ConsoleApp → Application → Domain ← Infrastructure

## Kural

```
Bağımlılıklar her zaman içe doğru akar.
Domain hiçbir dış katmanı tanımaz.
Infrastructure, Domain arayüzlerini uygular.
```

**Tehlike işaretleri:**
- `Program.cs` veya controller'da `new SmtpClient()`, `new DbContext()` çağrıları
- Domain entity'sinin `using System.Net.Mail` veya benzeri infrastructure namespace içermesi
- Tek `.csproj` dosyasında tüm uygulama
