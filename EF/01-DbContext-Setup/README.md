# 01 — DbContext Temel Kurulum & Konfigürasyon

## Ne Öğretir?

`DbContext`'in nasıl doğru yapılandırılacağını, dependency injection ile nasıl kullanılacağını ve connection string yönetiminin nasıl yapılması gerektiğini gösterir.

## Neden Önemli?

`DbContext` EF Core'un kalbidir. Yanlış yapılandırma:
- **Memory leak**'e yol açar (dispose edilmeyen context'ler)
- **Thread-safety** sorunları yaratır (singleton context)
- **Güvenlik riski** oluşturur (hardcode connection string)
- **Test edilemez** kod üretir (new ile oluşturulan context)

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `new AppDbContext()` | DI kullanılmıyor, her yerde manuel oluşturuluyor |
| Hardcode connection string | `OnConfiguring` içinde sabit yazılmış |
| Dispose edilmiyor | Memory leak riski |
| `OnConfiguring` konfigürasyonu | DI uyumsuz, test edilemez |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `AddDbContext<T>` | Scoped lifetime, framework yönetir |
| `appsettings.json` | Connection string ortama göre değişebilir |
| Constructor injection | Test edilebilir, mock'lanabilir |
| `IDesignTimeDbContextFactory` | Migration'lar için design-time desteği |
| Fluent API | Model konfigürasyonu merkezi ve okunabilir |

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
# Ürün listesi
GET http://localhost:5000/products

# Ürün ekle
POST http://localhost:5000/products
Content-Type: application/json
{ "name": "Laptop", "price": 15000 }
```
