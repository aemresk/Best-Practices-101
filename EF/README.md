# Entity Framework Core — Best Practices 101

.NET 8 + Minimal API + SQLite ile hazırlanmış EF Core best practice örnekleri.

## Örnekler

| # | Konu | Klasör |
|---|------|--------|
| 01 | DbContext Temel Kurulum & Konfigürasyon | [01-DbContext-Setup](01-DbContext-Setup/) |
| 02 | AsNoTracking — Read-only Sorgular | [02-AsNoTracking](02-AsNoTracking/) |
| 03 | Eager / Lazy / Explicit Loading | [03-Loading-Strategies](03-Loading-Strategies/) |
| 04 | N+1 Sorunu ve Çözümü | [04-N-Plus-1](04-N-Plus-1/) |
| 05 | Repository Pattern | [05-Repository-Pattern](05-Repository-Pattern/) |
| 06 | Migration Best Practices | [06-Migration-Best-Practices](06-Migration-Best-Practices/) |
| 07 | Transaction Yönetimi | [07-Transaction-Management](07-Transaction-Management/) |
| 08 | Pagination (Sayfalama) | [08-Pagination](08-Pagination/) |
| 09 | Soft Delete Pattern | [09-Soft-Delete](09-Soft-Delete/) |
| 10 | Connection Resiliency & Retry Policy | [10-Connection-Resiliency](10-Connection-Resiliency/) |

## Yapı

Her örnek şu yapıyı takip eder:

```
XX-Konu-Adi/
├── README.md        ← Açıklama, neden önemli, nasıl çalıştırılır
├── Before/          ← Yaygın hatalar
└── After/           ← Doğru yaklaşım
```
