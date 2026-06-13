# 07 — Domain Events

> Domain'de gerçekleşen önemli olaylar, event olarak yayınlanmalı; yan etkiler ayrı handler'larda olmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Processing](Before/) | `Order.Place` içinde e-posta, stok, audit doğrudan çağrılıyor |

## Fark

| | Before | After |
|--|--------|-------|
| Yan etki ekleme | `Order` sınıfı değiştirilmeli | Yeni handler register edilir, `Order` dokunulmaz |
| Bağımlılık | `Order(EmailNotifier, AuditLog, Inventory)` | `Order()` — sıfır bağımlılık |
| Test | E-posta göndermeden test edilemez | Domain event kontrol edilir, handler ayrı test edilir |
| Sıralama | Hardcode — her zaman aynı sıra | Handler'lar bağımsız, sıra değiştirilebilir |

## Kural

```
Domain, "ne oldu" der — event yayar.
Handler'lar "ne yapılacak" der — yan etkileri yönetir.
```

**Tehlike işaretleri:**
- Domain entity constructor'ı service/notifier alıyor
- `Order.Place()`, `Order.Cancel()` gibi metodların içinde `Console.WriteLine` değil gerçek IO çağrısı var
- Yeni bir yan etki eklemek için domain sınıfını değiştiriyorsunuz
