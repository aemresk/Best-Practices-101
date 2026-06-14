# 04 — CancellationToken

> Her async metod `CancellationToken` parametresi almalı ve onu tüm alt çağrılara iletmeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Report Generation](Before/) | `GenerateReportAsync` ve tüm alt metodları token almıyor |

## Fark

| | Before | After |
|--|--------|-------|
| İptal | Mümkün değil — process sonlanana kadar çalışır | `cts.Cancel()` veya timeout ile anında durur |
| Kaynak tasarrufu | Gereksiz DB/HTTP çağrıları tamamlanır | İptal noktasında durur |
| Zaman aşımı | Manuel `Task.Delay` + flag gerekir | `new CancellationTokenSource(TimeSpan)` |
| ASP.NET Core | İstek iptalini bilemez | `HttpContext.RequestAborted` token olarak geçilir |

## Kural

```
Her async metodun son parametresi: CancellationToken ct = default
Token her alt çağrıya iletilir: await SomethingAsync(ct)
Uzun döngülerde: ct.ThrowIfCancellationRequested()
```

**Tehlike işaretleri:**
- `async Task DoWorkAsync()` — parametre listesinde `CancellationToken` yok
- `await Task.Delay(5000)` — token geçilmeden uzun gecikme
- ASP.NET Core action metodunda `CancellationToken` parametresi yok
