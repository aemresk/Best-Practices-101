# .NET Performance Best Practices

> Tek çalışan demo projesi — 10 performans konusu gerçek ölçümlerle gösterilir.
> Her demo: **süre (ms) | toplam allocation (KB) | GC Gen0 sayısı**

## Çalıştırma

```bash
cd Performance/PerfDemo

# Demo modu — tüm konuları sırayla çalıştırır (~30 saniye)
dotnet run -c Release

# BenchmarkDotNet modu — istatistiksel benchmark (birkaç dakika)
dotnet run -c Release -- --benchmark
```

> ⚠️ Debug modunda (`dotnet run`) JIT optimizasyonları kapalıdır — sayılar aldatıcı olabilir.

---

## Konu Listesi

| # | Konu | Anahtar Teknik | Dosya |
|---|------|----------------|-------|
| 01 | Span\<T\> | `AsSpan()`, `Slice`, sıfır kopya | `Demos/01_SpanDemo.cs` |
| 02 | ArrayPool\<T\> | `Rent` / `Return`, LOH kaçınma | `Demos/02_ArrayPoolDemo.cs` |
| 03 | stackalloc | `Span<byte> buf = stackalloc byte[N]` | `Demos/03_StackallocDemo.cs` |
| 04 | String Ops | `StringBuilder`, `Contains(char)`, `OrdinalIgnoreCase`, `string.Create` | `Demos/04_StringOpsDemo.cs` |
| 05 | struct vs class | `readonly struct`, `in` parametresi, cache locality | `Demos/05_StructDemo.cs` |
| 06 | ObjectPool\<T\> | `DefaultObjectPoolProvider`, `Get`/`Return` | `Demos/06_ObjectPoolDemo.cs` |
| 07 | LINQ vs for | Enumerator overhead, allocation, erken çıkış | `Demos/07_LinqVsLoopDemo.cs` |
| 08 | FrozenDictionary | `ToFrozenDictionary()`, mükemmel hash | `Demos/08_FrozenColDemo.cs` |
| 09 | Lazy\<T\> | `Lazy<T>`, `LazyInitializer`, geciktirilmiş başlatma | `Demos/09_LazyInitDemo.cs` |
| 10 | BenchmarkDotNet | `[MemoryDiagnoser]`, `[Benchmark]`, istatistiksel ölçüm | `Demos/10_BdnDemo.cs` |

---

## 01 — Span\<T\>: Sıfır Kopya Dilim İşlemleri

`ReadOnlySpan<char>` heap allocate etmez — stack'te bir dilim referansıdır.

```csharp
// ❌ Her Substring yeni bir string nesnesi oluşturur
string city = csv.Substring(9, 6);          // heap alloc

// ✅ Span: orijinal belleğe pencere açar, kopya yok
ReadOnlySpan<char> city = csv.AsSpan(9, 6); // sıfır alloc
```

**CSV parsing farkı:** `Split(',')[0]` → 734 MB allocation vs `AsSpan + IndexOf + Slice` → 2 KB

**Kural:** Span\<T\> `ref struct`'tır — async metotlarda ve field olarak kullanılamaz; kısa ömürlü sync işlemler için idealdir.

---

## 02 — ArrayPool\<T\>: GC Baskısını Azalt

Her işlemde `new byte[]` yerine havuzdan kirala, işi bitince iade et.

```csharp
// ❌ GC takip eder, 85KB+ LOH'a gider
var buf = new byte[4096];

// ✅ Havuzdan al, finally'de mutlaka iade et
var buf = ArrayPool<byte>.Shared.Rent(4096);
try   { /* kullan */ }
finally { ArrayPool<byte>.Shared.Return(buf); }
```

**LOH senaryosu:** 50K × `new byte[100KB]` → 5 GB allocation, 1612 GC0 vs `ArrayPool` → 8 KB, 0 GC.

---

## 03 — stackalloc: Stack Bellek Tahsisi

Küçük geçici buffer'lar için heap yerine stack'i kullan.

```csharp
// ❌ GC nesnesi
var buf = new byte[128];

// ✅ Stack frame'de yaşar, GC görmez
Span<byte> buf = stackalloc byte[128];

// Sayı formatlama — sıfır alloc
Span<char> chars = stackalloc char[32];
myInt.TryFormat(chars, out int written, "N0");
```

**Kural:** ≤ ~512 byte için uygundur. Büyük `stackalloc` → `StackOverflowException`.

---

## 04 — String Optimizasyonları

| Anti-pattern | Optimizasyon | Kazanç |
|---|---|---|
| `s += "parça"` döngüde | `StringBuilder.Append` | Allocation: 272MB → 21MB |
| `str.Contains("r")` | `str.Contains('r')` | Char overload daha hızlı |
| `a.ToLower() == b.ToLower()` | `OrdinalIgnoreCase` | 566ms + 468MB → 35ms + 0B |
| `$"PRD-{id:D6}"` | `string.Create(...)` | Tek seferlik buffer yazma |

```csharp
// OrdinalIgnoreCase: kültür bağımsız, sıfır allocation
string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

// string.Create: hedef buffer'a doğrudan yaz
string.Create(10, id, static (span, v) =>
{
    "PRD-".CopyTo(span);
    v.TryFormat(span[4..], out _, "D6");
});
```

---

## 05 — struct vs class: Heap Baskısını Azalt

`struct[]` → tek kontiguous bellek bloğu; `class[]` → her eleman ayrı heap nesnesi.

```csharp
// ✅ readonly struct: defensive copy engellenir
private readonly struct Point(double x, double y)
{
    public readonly double X = x;
    public readonly double Y = y;

    // in: büyük struct'ı kopyalamadan geç
    public double DistanceTo(in Point other) => ...;
}
```

**50K nokta dizisi oluşturma:** `PointClass[]` → 421ms + 976MB vs `PointStruct[]` → 47ms + 390MB

---

## 06 — ObjectPool\<T\>: Pahalı Nesneleri Yeniden Kullan

`StringBuilder`, `MemoryStream` gibi pahalı nesneleri havuzla.

```csharp
// Singleton pool — başlangıçta bir kez oluştur
private static readonly ObjectPool<StringBuilder> _pool =
    new DefaultObjectPoolProvider().CreateStringBuilderPool(256, 4096);

var sb = _pool.Get();
try
{
    sb.Append("..."); // kullan
    return sb.ToString();
}
finally
{
    _pool.Return(sb); // policy otomatik Clear() çağırır
}
```

---

## 07 — LINQ vs for Döngüsü

LINQ okunabilirlik için mükemmeldir; hot path'te fark yaratır.

```csharp
// ❌ Her çağrıda List alloc + lambda overhead
list.Where(x => x > 5000).ToList();

// ✅ Hot path'te tek alloc
var result = new List<int>(list.Count / 2);
foreach (var x in list)
    if (x > 5000) result.Add(x);
```

**Kural:** LINQ'u tek seferlik veya okunabilirliğin kritik olduğu yerlerde kullan. Nanosaniye önemli olduğunda `for` tercih et.

---

## 08 — FrozenDictionary / FrozenSet (.NET 8+)

Startup'ta bir kez oluşturulan salt-okunur lookup tabloları için mükemmel hash hesaplar.

```csharp
// Startup'ta bir kez
var frozen = myDict.ToFrozenDictionary();

// Hot path'te — optimize edilmiş lookup
frozen.TryGetValue(key, out var value);
```

**Ne zaman:** Hata kodları, konfigürasyon haritaları, ülke/para birimleri gibi sabit tablolar.

---

## 09 — Lazy\<T\>: Geciktirilmiş Başlatma

Pahalı kaynakları yalnızca gerçekten ihtiyaç duyulduğunda oluştur.

```csharp
// Global singleton — ilk Value erişiminde başlatılır, thread-safe
private static readonly Lazy<ExpensiveService> _service =
    new(() => new ExpensiveService());

// Kullanım
var svc = _service.Value;
```

**Kural:** `Lazy<T>` varsayılan olarak `ExecutionAndPublication` mode — thread-safe. Factory exception fırlatırsa her `Value` erişiminde yeniden fırlatır.

---

## 10 — BenchmarkDotNet

Bu projedeki `Stopwatch + GC` ölçümü hızlı geliştirme geri bildirimi için yeterlidir. Karar vermeden önce gerçek benchmark al:

```bash
dotnet run -c Release -- --benchmark
```

```csharp
[MemoryDiagnoser]
[RankColumn]
public class MyBenchmarks
{
    [Benchmark(Baseline = true)]
    public string WithSubstring() => text.Substring(0, 10);

    [Benchmark]
    public ReadOnlySpan<char> WithSpan() => text.AsSpan(0, 10);
}
```

| Method | Mean | Ratio | Allocated |
|--------|------|-------|-----------|
| WithSubstring | 12.34 ns | 1.00 | 40 B |
| WithSpan | 1.23 ns | 0.10 | 0 B |

---

## Ölçüm Metodolojisi

`DemoRunner.Compare()` her senaryo için:
1. **Warmup** (3 iterasyon) — JIT derlenmesi beklenir
2. `GC.Collect(2)` — temiz başlangıç
3. `GC.GetTotalAllocatedBytes()` öncesi / sonrası → toplam KB
4. `GC.CollectionCount(0)` öncesi / sonrası → Gen0 GC sayısı
5. `Stopwatch` ile süre
