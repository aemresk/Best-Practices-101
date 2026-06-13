# 10 — Connection Resiliency & Retry Policy

## Ne Öğretir?

Geçici veritabanı bağlantı hatalarının (transient failures) nasıl otomatik olarak yeniden deneneceğini, `EnableRetryOnFailure` konfigürasyonunu ve explicit transaction ile retry'ın birlikte nasıl kullanılacağını gösterir.

## Neden Önemli?

Bulut veritabanları (Azure SQL, AWS RDS, Google Cloud SQL) periyodik olarak geçici kesintiler yaşar:
- Maintenance window (1-5 saniye)
- Otomatik failover
- Bağlantı havuzu dolması
- Ağ paket kaybı

Bu hatalar **geçicidir** — birkaç milisaniye sonra kendiliğinden çözülür. Retry olmadan uygulama hemen 500 döner. Retry ile müşteri hiçbir şey fark etmez.

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `EnableRetryOnFailure` yok | Geçici hata → direkt 500 dönüyor |
| Timeout ayarlanmamış | Varsayılan 30 saniye; yük altında kullanıcı bekler |
| Explicit TX + retry uyumsuz | `BeginTransactionAsync()` ile retry doğrudan çalışmaz |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `EnableRetryOnFailure(5, 30s, null)` | 5 deneme, exponential backoff, EF'in bilinen geçici hata listesi |
| `ExecutionStrategy.ExecuteAsync()` | Explicit transaction + retry için gerekli sarmalayıcı |
| `SetCommandTimeout(120s)` | Uzun sorgular için özel timeout — endpoint bazlı |

## Önemli Kısıtlama

> **SQLite `EnableRetryOnFailure` desteklemez.**
>
> Bu örnek demo için SQLite ile çalışır, ama retry konfigürasyonu SQL Server veya PostgreSQL'de aktif edilir. Gerçek uygulamada `UseSqlServer(...)` veya `UseNpgsql(...)` ile kullanın.

## Retry + Explicit Transaction

```
Yanlış:
    BeginTransactionAsync()
    retry → ilk SaveChanges tekrar çalışır ama transaction kapandı → exception

Doğru:
    strategy.ExecuteAsync(async () => {
        BeginTransactionAsync()
        SaveChangesAsync()
        CommitAsync()
    })
    → tüm blok yeniden deneniyor, transaction her seferinde taze açılıyor
```

## Nasıl Çalıştırılır?

```bash
# Before
cd Before && dotnet run

# After (SQLite ile demo)
cd After && dotnet run
```

```bash
GET  http://localhost:5000/orders
POST http://localhost:5000/orders
Content-Type: application/json
{ "customerName": "Ali", "amount": 1500 }

POST http://localhost:5000/orders/with-explicit-tx
Content-Type: application/json
{ "customerName": "Ayşe", "amount": 2200 }

GET  http://localhost:5000/orders/report
```
