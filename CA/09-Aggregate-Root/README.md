# 09 — Aggregate Root

> İlişkili entity'ler bir Aggregate Root üzerinden yönetilmeli; dışarıdan doğrudan erişim engellenmeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Shopping Cart](Before/) | `CartService` `cart.Items` listesini doğrudan manipüle ediyor |

## Fark

| | Before | After |
|--|--------|-------|
| `Items` erişimi | `public List<CartItem>` — herkes `Add/Remove` yapabilir | `IReadOnlyList<CartItem>` — okuma only |
| Kural konumu | `CartService.AddItem` — başka servis atlayabilir | `Cart.AddItem` — atlanamaz |
| Duplicate kontrol | Serviste (unutulabilir) | Root'ta (her zaman çalışır) |
| `CartItem` oluşturma | `new CartItem { ... }` — geçersiz değer mümkün | `CartItem.Create(...)` internal — sadece `Cart` çağırabilir |
| Geçersiz durum | `cart.Items.Add(geçersizItem)` ← derlenir | Derleme hatası: `IReadOnlyList.Add` yok |

## Kural

```
Aggregate Root, sınırları içindeki entity'lerin tek kapısıdır.
Dış dünya yalnızca Root'un public metodlarıyla konuşur.
```

**Tehlike işaretleri:**
- Entity'de `public List<ChildEntity> Children { get; set; }`
- `ChildEntity` constructor'ı `public` ve ayrı servisten doğrudan `new`'leniyor
- İş kuralları entity değil, servis sınıfında uygulanıyor
