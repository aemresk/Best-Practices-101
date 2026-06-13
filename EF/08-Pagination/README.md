# 08 — Pagination (Sayfalama)

## Ne Öğretir?

Sayfalama işleminin SQL katmanında yapılmasını, `PagedResult<T>` ile tutarlı bir response yapısını ve offset pagination ile cursor pagination arasındaki farkı gösterir.

## Neden Önemli?

Sayfalama olmadan veya yanlış yapılınca:
- Tüm tablo belleğe çekilir — 1 milyon kayıtta uygulama çöker
- `SELECT COUNT(*) `için `ToListAsync()` çağrılır — gereksiz veri transferi
- `OrderBy` olmadan sayfa içeriği tutarsız döner — aynı sayfa farklı sonuç verebilir
- İstemci `pageSize=100000` gönderirse sunucu kabul eder

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `ToListAsync()` → `Skip/Take` | Sayfalama C#'ta — tüm veri önce belleğe alınıyor |
| Sayfalama yok | `/products/all` sınırsız kayıt döndürüyor |
| Count için `ToListAsync()` | Sadece sayı için tüm satırlar çekiliyor |
| `OrderBy` eksik | Her sorguda farklı sıralama — tutarsız sayfalar |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `Skip/Take` sorguda | SQL: `LIMIT N OFFSET M` — sadece o sayfa gelir |
| `CountAsync()` ayrı | SQL: `SELECT COUNT(*)` — veri taşımadan sayar |
| `Math.Clamp(pageSize, 1, 100)` | İstemci büyük sayı gönderemez |
| `PagedResult<T>` | Tutarlı response: items + totalCount + sayfa bilgisi |
| Cursor-based endpoint | Büyük veri setlerinde offset'ten O(1) daha hızlı |

## Offset vs Cursor Pagination

| | Offset | Cursor |
|---|---|---|
| **Nasıl** | `SKIP (page-1)*size TAKE size` | `WHERE id > lastId TAKE size` |
| **Avantaj** | Rasgele sayfaya atlama mümkün | Büyük veri setinde sabit performans |
| **Dezavantaj** | Derin sayfalarda DB tüm offset'i sayar | Önceki sayfaya geri gidilemez |
| **Ne zaman** | < 100K kayıt, kullanıcı sayfası atlar | Feed, infinite scroll, büyük tablo |

## Nasıl Çalıştırılır?

```bash
# Before
cd Before
dotnet run

# After
cd After
dotnet run
```

```bash
# Offset pagination
GET http://localhost:5000/products?page=1&pageSize=10
GET http://localhost:5000/products?page=2&pageSize=5

# Kategori filtreli
GET http://localhost:5000/products/by-category?category=Elektronik&page=1

# Cursor-based (sadece After)
GET http://localhost:5000/products/cursor?pageSize=10
GET http://localhost:5000/products/cursor?lastId=10&pageSize=10
```

Cursor endpoint'in response'ındaki `nextCursor` değerini sonraki isteğe `lastId` olarak gönderin.
