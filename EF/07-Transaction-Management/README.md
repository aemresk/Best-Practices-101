# 07 — Transaction Yönetimi

## Ne Öğretir?

EF Core'un transaction modelini, çoklu `SaveChanges` çağrısının yarattığı tutarsızlık riskini ve explicit transaction'ın ne zaman gerçekten gerekli olduğunu gösterir.

## Neden Önemli?

Transaction, birden fazla işlemi **ya hepsini ya hiçbirini** ilkesiyle bağlar. Olmadığında:
- Sipariş oluşturulur ama stok düşürülmez
- Stok düşürülür ama ödeme kaydedilmez
- Veritabanı tutarsız bir ara durumda kalır — bu hata değil, sessiz veri bozukluğudur

## Temel Kural

```
Tek SaveChangesAsync()  → EF Core otomatik transaction açar, kapsar, commit eder ✅
Birden fazla SaveChangesAsync() → Her biri bağımsız commit → tutarsızlık riski ❌
```

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| Döngü içinde `SaveChangesAsync` | Her ürün ayrı commit — 3. üründe hata → 1. ve 2. stok zaten düşürüldü |
| `SaveChanges` #1 Order, #2 Items, #3 Payment | Payment commit başarısız → sipariş veritabanında ama ödeme yok |
| Hata sonrası rollback yok | `catch` bloğu yok veya exception yutulmuş |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| Tek `SaveChangesAsync()` | Tüm değişiklikler ChangeTracker'a birikmesi, tek commit |
| `BeginTransactionAsync()` | Birden fazla SaveChanges zorunluysa explicit transaction |
| `CommitAsync()` / `RollbackAsync()` | try/catch ile hata durumunda rollback garantisi |

## Ne Zaman Explicit Transaction?

**Gerekmez** (çoğu durum):
- Tüm değişiklikler tek `SaveChangesAsync()` ile commit ediliyorsa — EF transaction'ı otomatik yönetir

**Gerekir** (nadir durumlar):
- Aynı işlemde birden fazla `SaveChangesAsync()` çağrısı zorunluysa
- Dış servis çağrısı (e-posta, ödeme API) veritabanı operasyonuyla aynı transaction'a alınmak isteniyorsa
- Farklı DbContext instance'ları aynı işlemi paylaşıyorsa (`TransactionScope`)

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
# Sipariş oluştur (tek SaveChanges — atomik)
POST http://localhost:5000/orders
Content-Type: application/json
{
  "customerName": "Ali",
  "items": [
    { "productId": 1, "quantity": 1 },
    { "productId": 2, "quantity": 2 }
  ]
}

# Siparişleri listele
GET http://localhost:5000/orders

# Explicit transaction örneği (sadece After)
POST http://localhost:5000/orders/1/confirm
```
