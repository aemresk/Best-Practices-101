# 08 — Exception Handling in Async

> `Task.WhenAll` tüm exception'ları taşır ama `catch` bloğu yalnızca ilkini görür; `task.Exception.InnerExceptions` ile tümüne ulaşılmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Parallel Calls](Before/) | WhenAll'da B ve C exception'ları kaybolur; fire-and-forget exception yutar |

## Fark

| | Before | After |
|--|--------|-------|
| WhenAll exception | `catch (Exception ex)` — sadece ilki | `task.Exception.InnerExceptions` — hepsi |
| Fire-and-forget | `_ = DoWorkAsync()` — sessiz kayıp | `FireAndForget(task)` — loglanır |
| `Task.Run` exception | `catch (AggregateException)` | `await` + `catch (ActualType)` |
| Kısmi başarı | Bilinmiyor | `ContinueWith` + her Task ayrı kontrol |

## Kural

```
WhenAll + birden fazla hata → await önce Task'ı sakla, sonra task.Exception.InnerExceptions oku
Fire-and-forget → async void FireAndForget(Task) yardımcısı ile logla
await Task.Run → AggregateException değil, asıl tipi yakalar
```

**Tehlike işaretleri:**
- `_ = SomeAsync()` — Task ignore ediliyor
- `Task.WhenAll(...)` sonucu catch'te sadece `ex.Message` yazılıyor (diğerleri kayıp)
- `.Wait()` ile `AggregateException` yakalanıyor, `InnerException` kontrol edilmiyor
