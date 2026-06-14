# 10 — SemaphoreSlim (Async-Safe Locking)

> Async metodlarda `lock` kullanılamaz — bunun yerine `SemaphoreSlim.WaitAsync()` kullanılmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Request Counter](Before/) | `lock` + `await` derleme hatası → ya race condition ya blocking |

## Neden `lock` Async'te Çalışmaz?

- `lock` bloğunun içinde `await` yazılırsa: **CS1996 derleme hatası**
- `lock` ile `Thread.Sleep`: thread pool thread bloke → async faydası sıfır
- `lock` olmadan `await` + shared state: **race condition**

## Fark

| | `lock` | `SemaphoreSlim` |
|--|--------|-----------------|
| `await` içinde | ❌ derleme hatası | ✅ `await WaitAsync()` |
| Thread block | Thread bloke | Serbest — başkası çalışır |
| Eş zamanlı limit | Yalnızca 1 | `new SemaphoreSlim(N, N)` ile N |
| `finally` | Gerekli değil | **Zorunlu** — Release her zaman çağrılmalı |

## Kural

```csharp
// ✅ Async-safe mutex:
private readonly SemaphoreSlim _lock = new(1, 1);

await _lock.WaitAsync();
try   { /* kritik bölge */ }
finally { _lock.Release(); }   // ASLA unutma
```

**Tehlike işaretleri:**
- `lock (_obj) { await ...; }` — CS1996 derleme hatası
- Shared mutable state'e `lock` olmadan async metoddan erişim
- `SemaphoreSlim.Release()` `finally` bloğunun dışında — exception durumunda deadlock
