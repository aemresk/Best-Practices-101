# ASP.NET Core API Best Practices

> Tek çalışan Minimal API projesi — 9 best practice bir arada, Products domain'i üzerinden gösterilir.

## Uygulanan Best Practice'ler

| # | Konu | Nerede |
|---|------|--------|
| 01 | Global Exception Handling (ProblemDetails) | `Common/Errors/GlobalExceptionHandler.cs` |
| 02 | FluentValidation | `Common/Filters/ValidationFilter.cs` + `Features/Products/ProductValidator.cs` |
| 03 | Result Pattern | `Common/Errors/Result.cs` + `Features/Products/ProductService.cs` |
| 04 | API Versioning | `Program.cs` → `MapGroup + WithApiVersionSet` |
| 05 | Rate Limiting | `Extensions/RateLimitingExtensions.cs` |
| 06 | Response Caching | `Extensions/CachingExtensions.cs` → `AddResponseCaching` |
| 07 | Health Checks | `Extensions/HealthCheckExtensions.cs` |
| 08 | Custom Middleware | `Common/Middleware/RequestLoggingMiddleware.cs` |
| 09 | Output Cache & IMemoryCache | `Extensions/CachingExtensions.cs` + `ProductService.cs` |

## Nasıl Çalıştırılır?

```bash
cd ASPNET/BestPracticesApi
dotnet run
# API → http://localhost:5000
```

## Endpoint'ler

### v1 — tam CRUD
```
GET    /api/v1/products        → tüm ürünler (output cache 30s)
GET    /api/v1/products/{id}   → tekil ürün
POST   /api/v1/products        → yeni ürün (validation + strict rate limit)
PUT    /api/v1/products/{id}   → güncelle   (validation)
DELETE /api/v1/products/{id}   → sil
```

### v2 — salt-okunur + sayfalama
```
GET    /api/v2/products        → sayfalı liste  (?page=1&pageSize=10)
GET    /api/v2/products/{id}   → genişletilmiş model (CreatedAt, Category)
```

### Health Checks
```
GET    /health          → tüm check'ler
GET    /health/live     → Kubernetes liveness probe  (tag: live)
GET    /health/ready    → Kubernetes readiness probe (tag: ready)
```

---

## 01 — Global Exception Handling

Tüm işlenmeyen exception'lar `GlobalExceptionHandler` tarafından yakalanır ve **RFC 9457 ProblemDetails** formatında döner.

```csharp
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// ...
app.UseExceptionHandler();
```

```json
// 500 yanıtı:
{
  "status": 500,
  "title": "Sunucu hatası",
  "detail": "Beklenmeyen bir hata oluştu.",
  "instance": "/api/v1/products"
}
```

**Kural:** Exception tipine göre `status` otomatik belirlenir: `ArgumentException` → 400, `KeyNotFoundException` → 404, diğerleri → 500.

---

## 02 — FluentValidation

`ValidationFilter<T>` endpoint filter olarak çalışır. Endpoint'e eklenmesi yeterli:

```csharp
group.MapPost("/", Create)
     .AddEndpointFilter<ValidationFilter<CreateProductRequest>>();
```

Doğrulama başarısız olduğunda **RFC 7807 ValidationProblem** döner:

```json
{
  "errors": {
    "Name": ["Ürün adı zorunlu"],
    "Price": ["Fiyat 0'dan büyük olmalı"]
  }
}
```

---

## 03 — Result Pattern

Servisler exception fırlatmaz; `Result<T>` döner. Endpoint bunu HTTP yanıtına çevirir:

```csharp
// Servis
public Result<Product> GetById(int id)
{
    return _store.TryGetValue(id, out var product)
        ? Result.Success(product)
        : Result.Failure<Product>(AppError.NotFound("Product"));
}

// Endpoint
private static IResult GetById(int id, ProductService service) =>
    service.GetById(id).ToHttpResult();
```

`ToHttpResult()` → `*.NotFound` kodu → `404 Not Found`, `*.Conflict` → `409 Conflict`.

---

## 04 — API Versioning

URL segment versiyonlama: `/api/v{version}/products`

```csharp
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .Build();
```

- v1 ve v2 aynı `ProductService`'i paylaşır
- v2 `PagedResult<ProductV2>` ile `CreatedAt` ve `Category` alanı ekler
- Yanıtta `api-supported-versions: 1, 2` header'ı döner

---

## 05 — Rate Limiting

| Policy | Limit | Kapsam |
|--------|-------|--------|
| Global | 10 istek / 10 sn per-IP | Tüm endpoint'ler |
| `strict` | 3 istek / 5 sn | POST (ürün oluştur) |

429 yanıtında `Retry-After` header'ı döner.

---

## 06 — Response Caching

`AddResponseCaching()` + `UseResponseCaching()` ile aktif. Endpoint Cache-Control header gönderirse istemci ve proxy cache yapar:

```csharp
// Endpoint'te manuel kullanım örneği:
context.Response.Headers.CacheControl = "public,max-age=60";
```

Bu projede Output Cache (09) tercih edilmiştir; ikisi birlikte de çalışabilir.

---

## 07 — Health Checks

Özel health check — bellek eşiği izleme:

```csharp
public Task<HealthCheckResult> CheckHealthAsync(...)
{
    var allocatedMb = GC.GetTotalMemory(false) / 1024 / 1024;
    return allocatedMb < 512
        ? HealthCheckResult.Healthy(...)
        : HealthCheckResult.Degraded(...);
}
```

Yanıt formatı:

```json
{
  "status": "Healthy",
  "duration": 1.23,
  "checks": [
    { "name": "api",    "status": "Healthy", "description": "API çalışıyor" },
    { "name": "memory", "status": "Healthy", "description": "Bellek normal: 8 MB" }
  ]
}
```

---

## 08 — Custom Middleware

`RequestLoggingMiddleware` pipeline'da tüm istekleri yakalar:

```csharp
app.UseMiddleware<RequestLoggingMiddleware>();
```

Çıktı:

```
[GET] /api/v1/products → 200 (3ms)
[POST] /api/v1/products → 201 (12ms)
[GET] /api/v1/products/999 → 404 (1ms)
```

5xx yanıtlarda `LogLevel.Error`, diğerlerinde `LogLevel.Information` kullanır.

---

## 09 — Output Cache & IMemoryCache

**İki katmanlı cache:**

| Katman | Ne için | Nerede |
|--------|---------|--------|
| `IMemoryCache` | Servis seviyesi — `GetAll()` sonucu 30s | `ProductService.cs` |
| Output Cache | HTTP yanıtı tamponlama — `GET /products` 30s | `ProductEndpoints.cs` + policy |

**Output Cache invalidation:** POST/PUT/DELETE başarılı olduğunda `"products"` tag'li cache temizlenir:

```csharp
await outputCache.EvictByTagAsync("products", ct);
```

**Tehlike işaretleri:**
- `IMemoryCache`'i `Singleton` servislere enjekte etme (DI scope uyuşmazlığı riski)
- Output Cache'i invalidate etmeden mutasyon yapma (stale data)
- `IMemoryCache` için süre belirtmemek — bellek sızıntısı riski
