# 04 — Repository Abstraction

> Uygulama servisi, veriyi nasıl sakladığını bilmemeli; yalnızca bir arayüzle konuşmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Product Management](Before/) | Servis `List<Product>` (veya DbContext) doğrudan tutuyor |

## Fark

| | Before | After |
|--|--------|-------|
| Depolama | `private readonly List<Product> _db` serviste | `IProductRepository` arayüzü inject ediliyor |
| Değiştirme | List → EF geçince servis sınıfı değişmeli | `InMemory → EfProductRepository` sadece DI'da değişir |
| Unit test | Gerçek List gerekiyor | Mock `IProductRepository` yeterli |
| İş kuralı | `product.Stock -= sold` servis içinde | `product.Sell(quantity)` — kural entity'de |

## Kural

```
Servis, depolama teknolojisini bilmez.
Repository arayüzü Domain'de, implementasyon Infrastructure'da olur.
```

**Tehlike işaretleri:**
- Servis sınıfında `new List<>()`, `new DbContext()`, `File.ReadAllText()` çağrıları
- Servis sınıfı `using Microsoft.EntityFrameworkCore` içeriyor
- Depolama değiştiğinde birden fazla servis sınıfı güncellenmek zorunda kalınıyor
