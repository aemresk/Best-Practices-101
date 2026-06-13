# 06 — CQRS (Command Query Responsibility Segregation)

> Okuma ve yazma operasyonları farklı modeller, farklı handler'lar kullanmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Product Catalog](Before/) | Tek `ProductService` hem `Create/Update` hem `GetAll/GetSummary` içeriyor |

## Fark

| | Before | After |
|--|--------|-------|
| Model | Tek `Product` record — hem read hem write | Write: domain entity / Read: `ProductListItem` DTO |
| Servis | `ProductService` — tüm operasyonlar | `CreateProductHandler`, `UpdatePriceHandler`, `GetAllProductsHandler` |
| Read optimizasyonu | Write modeli return ediliyor | Query handler farklı projeksiyon üretir |
| Ölçekleme | Read yavaşlarsa write de etkilenir | Query handler'lar read replica'ya yönlendirilebilir |

## Kural

```
Command → state değiştirir, void veya sadece id döner.
Query   → state değiştirmez, her zaman bir projeksiyon döner.
```

**Tehlike işaretleri:**
- `GetById` metodu hem veri döndürüyor hem de `lastAccessedAt` güncelliyor
- Write operasyonları tam domain entity döndürüyor (id yeterli)
- Tek servis sınıfında 10+ metod: okuyanlar ve yazanlar iç içe
