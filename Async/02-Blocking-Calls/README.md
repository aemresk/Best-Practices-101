# 02 — Blocking Calls (.Result / .Wait() Deadlock)

> `.Result`, `.Wait()` ve `.GetAwaiter().GetResult()` thread'i bloke eder; SynchronizationContext olan ortamlarda deadlock'a yol açar.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Service](Before/) | `GetOrderAsync().Result` — thread bloke, deadlock riski |

## Deadlock Mekanizması

```
1. UI/ASP.NET thread → .Result çağrısı → thread bloke, SyncContext tutuluyor
2. await tamamlanır → continuation aynı SyncContext'te devam etmek ister
3. SyncContext bloke thread tarafından tutuluyor → DEADLOCK
```

## Fark

| | Before | After |
|--|--------|-------|
| Thread | Bloke — I/O boyunca bekler | Serbest — başka işlere bakabilir |
| Deadlock | SyncContext ortamında garanti | Yok |
| Throughput | N thread = N eş zamanlı istek | Çok daha az thread, çok daha fazla istek |
| Okunabilirlik | `.Result` zinciri karışık | `await` zinciri temiz |

## Kural

```
.Result   → yasak (sadece kesinlikle SyncContext olmayan yerde, son çare)
.Wait()   → yasak
.GetAwaiter().GetResult() → .Result ile özdeş, aynı risk
```

**Tehlike işaretleri:**
- Controller, service veya repository'de `.Result` veya `.Wait()` kullanımı
- `Task.Run(() => asyncMethod()).Result` deadlock'tan kaçınmak için yazılmış
- Library kodu `async` olmayan public metodda async çağırıyor
