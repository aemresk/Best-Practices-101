# 04 — N+1 Sorunu ve Çözümü

## Ne Öğretir?

N+1 probleminin nasıl oluştuğunu, nasıl **görünür** kılınacağını (SQL loglama) ve iki farklı türünün (döngüsel N+1 ve Cartesian Explosion) nasıl çözüleceğini gösterir.

## Neden Önemli?

N+1 bir hata değil, **sessiz bir performans sorunudur** — uygulama çalışır, veri doğru gelir, ama veritabanına gereğinden çok sorgu gider. 100 ürün için 101 sorgu yerine 1 sorgu yeterli olabilir.

## İki Farklı N+1 Türü

### 1. Döngüsel N+1
```
Sorgu 1: SELECT * FROM Products          → 5 satır
Sorgu 2: SELECT * FROM Reviews WHERE ProductId = 1
Sorgu 3: SELECT * FROM Reviews WHERE ProductId = 2
...
Sorgu 6: SELECT * FROM Reviews WHERE ProductId = 5
```
**5 ürün → 6 sorgu. 1000 ürün → 1001 sorgu.**

### 2. Cartesian Explosion (İki Collection Include)
```
Include(Reviews) + Include(Images) → tek JOIN sorgusu ama:
5 ürün × 3 yorum × 2 resim = 30 satır  (beklenen: 5+15+10 = 30 ama birleştirilmiş)
```
Veri doğru materialized edilir, ama ağ üzerinden taşınan veri miktarı şişer.

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `foreach` + per-item sorgu | N ürün için N+1 ayrı SQL gidiyor |
| İki collection'ı tek Include ile | `Include(Reviews).Include(Images)` → cartesian explosion |
| Her ortamda SQL loglama açık | Production'da `LogTo(Console.WriteLine)` → performans sorunu |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `Include()` | Döngüsel N+1'i JOIN ile tek sorguda çözer |
| `AsSplitQuery()` | İki collection'ı ayrı sorgularla çeker — cartesian explosion yok |
| `Select()` projection | Count/Average SQL'de hesaplanır — Include bile gerekmiyor |
| `LogTo` sadece Development'ta | Sorgu logları production'a sızmaz |

## Nasıl Gözlemleyebilirsiniz?

Her iki proje de konsola SQL sorguları yazdırır. Uygulamayı çalıştırıp endpoint'e istek atın:

```
Before /products/with-reviews  → 6 sorgu (5 ürün için)
After  /products/with-reviews  → 1 sorgu

Before /products/full          → 1 JOIN sorgusu ama 30 satır
After  /products/full          → 3 ayrı sorgu, temiz veri
After  /products/summary       → 1 sorgu, SQL'de hesaplanmış özet
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
GET http://localhost:5000/products/with-reviews
GET http://localhost:5000/products/full
GET http://localhost:5000/products/summary   # sadece After
```
