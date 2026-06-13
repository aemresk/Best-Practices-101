# 02 — AsNoTracking: Read-only Sorgular

## Ne Öğretir?

EF Core'un **ChangeTracker** mekanizmasının nasıl çalıştığını, neden maliyetli olduğunu ve okuma amaçlı sorgularda `AsNoTracking()` ile bu maliyetin nasıl ortadan kaldırılacağını gösterir.

## Neden Önemli?

EF Core, sorguladığı her entity'yi varsayılan olarak **ChangeTracker**'a ekler. Bu mekanizma `SaveChanges()` çağrıldığında değişen alanları tespit etmek için gereklidir. Ancak entity'yi **okuyup döndürüyorsanız ve değiştirmiyorsanız** bu maliyet tamamen gereksizdir:

- Snapshot oluşturulur (bellek)
- Her entity için identity map kaydı tutulur (bellek)
- `SaveChanges()` sırasında gereksiz change detection çalışır (CPU)

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `ToListAsync()` (tracking'li) | GET endpoint'lerde ChangeTracker devreye giriyor |
| `FindAsync` salt okunur için | Önce ChangeTracker'a bakar, sonra DB'ye gider — gereksiz adım |
| Gruplama uygulama katmanında | Tüm entity'ler belleğe çekilip C#'ta işleniyor |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `AsNoTracking()` | ChangeTracker devre dışı — daha az bellek, daha hızlı sorgu |
| `FirstOrDefaultAsync` + `AsNoTracking` | FindAsync yerine: doğrudan DB'ye gider |
| Projection + `AsNoTracking` | Gruplama/hesaplama SQL'e çevriliyor, uygulama katmanına ham veri taşınmıyor |
| Write'ta tracking | `Add`, `Remove`, `Update` için ChangeTracker hâlâ gerekli |

## Kural: Ne Zaman AsNoTracking?

```
Eğer entity'yi sadece okuyup döndürüyorsan → AsNoTracking()
Eğer entity'yi değiştireceksen            → tracking açık bırak
```

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
# Ürünleri kategorileriyle listele
GET http://localhost:5000/products

# Belirli bir ürün
GET http://localhost:5000/products/1

# Kategoriye göre rapor
GET http://localhost:5000/products/report

# Ürün ekle
POST http://localhost:5000/products
Content-Type: application/json
{ "name": "Klavye", "price": 750, "categoryId": 1 }

# Ürün sil
DELETE http://localhost:5000/products/1
```
