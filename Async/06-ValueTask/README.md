# 06 — ValueTask

> Senkron tamamlanmanın yaygın olduğu hot path metodlarında `Task` yerine `ValueTask` kullan.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Product Cache](Before/) | Cache hit'te `Task<Product>` döner — her seferinde heap allocation |

## Fark

| | `Task<T>` | `ValueTask<T>` |
|--|-----------|----------------|
| Senkron tamamlanma | Heap'te `Task` allocate | Stack'te struct — sıfır allocation |
| Gerçek async | Heap'te `Task` allocate | Heap'te `Task` allocate (aynı) |
| Birden fazla await | ✅ güvenli | ❌ yasak — sadece bir kez await edilir |
| Değişkende saklama | ✅ güvenli | ❌ riskli |
| Genel kural | Varsayılan tercih | Yalnızca profil gösterdiyse optimize et |

## Kural

```
Varsayılan: Task / Task<T>
ValueTask kullan → senkron tamamlanma yaygınsa VE profil baskı gösteriyorsa
ValueTask'ı birden fazla await etme, değişkende saklama
```

**Tehlike işaretleri:**
- Cache veya in-memory lookup yapan metodda `async Task<T>` döndürülüyor
- Bir `ValueTask`'ı `var t = GetAsync(); await t; await t;` şeklinde iki kez await etmek
- Her metodun dönüş tipini kör bir şekilde `ValueTask`'a dönüştürmek (profil yoksa gereksiz)
