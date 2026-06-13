# 03 — Eager / Lazy / Explicit Loading

## Ne Öğretir?

EF Core'un üç farklı veri yükleme stratejisini, ne zaman hangisinin kullanılacağını ve **N+1 problemini** gösterir.

## Neden Önemli?

Navigation property'ler (ilişkili tablolar) otomatik yüklenmez. Yanlış strateji seçimi:
- **N+1 problemi**: 100 ürün için 101 ayrı SQL sorgusu
- **Over-fetching**: Sadece ürün adı gereken yerde tüm ilişki grafiği yüklenir
- **NullReferenceException**: Include unutulunca navigation property null gelir

## Stratejiler

| Strateji | Nasıl | Ne Zaman |
|----------|-------|----------|
| **Eager Loading** | `Include()` / `ThenInclude()` | İlişkili veri her zaman gerektiğinde |
| **Explicit Loading** | `Entry().Collection().LoadAsync()` | İlişkili veri koşullu gerektiğinde |
| **Projection** | `Select(p => new { ... })` | Sadece belirli alanlar gerektiğinde |

> **Lazy Loading** (EF Core Proxies) kasıtlı olarak gösterilmemiştir. Navigation property'lere kod içinde erişildiğinde otomatik sorgu fırlatır — N+1 sorununu gizler, kontrol edilemez ve production'da tehlikelidir.

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| N+1 döngüsü | `ToListAsync()` + foreach içinde per-product sorgu |
| Her endpoint'te over-fetch | Sadece isim gereken yerde `Include(Category).Include(Reviews)` |
| Eksik Include | Navigation property `null` gelir — `?.` ile sessizce geçiştiriliyor |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `Include()` + `AsNoTracking()` | İlişkili veriler tek JOIN sorgusunda, tracking olmadan |
| `ThenInclude()` | Çok seviyeli ilişki: Category → Products → Reviews |
| `Explicit Loading` | `Entry().Collection().LoadAsync()` — koşula göre yükle |
| `Select()` projection | Navigation property yüklemeden SQL'de ilişki çözümlenir |

## Nasıl Çalıştırılır?

```bash
# Before
cd Before
dotnet run

# After
cd After
dotnet run
```

Uygulama ayağa kalktıktan sonra:

```bash
# N+1 vs Eager Loading karşılaştırması
GET http://localhost:5000/products/with-reviews

# Over-fetch vs Projection
GET http://localhost:5000/products/names

# Eksik Include vs Projection ile kategori adı
GET http://localhost:5000/products/with-category

# Explicit Loading (sadece After)
GET http://localhost:5000/products/1

# ThenInclude zinciri (sadece After)
GET http://localhost:5000/categories/full
```
