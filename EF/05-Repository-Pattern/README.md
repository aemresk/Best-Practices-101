# 05 — Repository Pattern

## Ne Öğretir?

Veri erişim mantığını endpoint'lerden ayırmanın, tekrar eden filtre kodunu tek bir yerde toplamanın ve **test edilebilir** kod yazmanın nasıl yapılacağını gösterir.

## Neden Önemli?

DbContext'i doğrudan endpoint'lere enjekte etmek işe yarar, ancak uygulama büyüdükçe sorunlar ortaya çıkar:

- **Tekrar eden filtre mantığı**: `Where(p => p.IsActive)` 4 farklı endpoint'e kopyalandı. Kuralı değiştirmek istesek hepsini bulmak gerekir.
- **Sıkı bağlantı**: Endpoint doğrudan EF Core'a bağlı — unit test yazarken gerçek veritabanı gerekir.
- **Dağınık iş kuralı**: "Aktif ürün nedir?" sorusunun cevabı kod tabanına yayılmış.

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `IsActive` filtresi tekrarlanıyor | 4 endpoint × aynı `Where` koşulu |
| DbContext endpoint'e direkt enjekte | Test için mock'lanamaz — gerçek DB şart |
| İş kuralı endpoint'te | "Aktif ürün" tanımı endpoint'lere gömülü |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `IProductRepository` interface | Endpoint'ler implementasyona değil sözleşmeye bağımlı |
| `ActiveProducts` private property | `IsActive` filtresi tek yerde — tüm metodlar buradan türüyor |
| Domain dilinde metodlar | `GetActiveAsync()`, `GetActiveCheaperThanAsync()` — ne yaptığı açık |
| `AddScoped<IProductRepository, ProductRepository>` | Test'te `FakeProductRepository` ile değiştirilebilir |

## Yapı

```
After/
├── Repositories/
│   ├── IProductRepository.cs   ← Sözleşme (interface)
│   └── ProductRepository.cs    ← EF Core implementasyonu
```

## Ne Zaman Repository Kullanmalısınız?

**Kullanın:**
- Aynı filtre/sorgu mantığı birden fazla yerde tekrarlanıyorsa
- Unit test yazacaksanız ve DB'yi mock'lamak istiyorsanız
- Veri erişim katmanını değiştirme ihtimaliniz varsa (EF Core → Dapper)

**Kullanmayın:**
- Basit CRUD uygulamalarında — DbContext zaten bir repository gibi davranır
- Her entity için boş bir `GetAll / GetById / Add / Delete` wrapper'ı oluşturmak için (Generic Repository anti-pattern)

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
GET  http://localhost:5000/products
GET  http://localhost:5000/products/cheap
GET  http://localhost:5000/categories/1/products
GET  http://localhost:5000/products/1
POST http://localhost:5000/products
Content-Type: application/json
{ "name": "Kulaklık", "price": 800, "categoryId": 1 }
DELETE http://localhost:5000/products/1
```
