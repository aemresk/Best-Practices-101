# 05 — Async All The Way

> Async zinciri hiç kırılmamalı — zincirin ortasında sync I/O async'in tüm faydasını yok eder.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [User Registration](Before/) | `SaveUserToDatabase` sync — 200ms thread bloke |

## Fark

| | Before | After |
|--|--------|-------|
| `SaveUserToDatabase` | `Thread.Sleep(200)` — thread bloke | `await Task.Delay(200)` — thread serbest |
| CPU işi (`HashPassword`) | Sync çağrı — doğrudan bloke | `await Task.Run(...)` — thread pool'a taşındı |
| Zincir | async → **sync** → async (kırık) | async → async → async (tam) |
| Throughput | Bloke eden her çağrı bir thread tutar | Thread pool thread'leri paylaşılır |

## Kural

```
I/O işlemleri: her zaman async versiyonunu kullan (ReadAsync, WriteAsync, QueryAsync…)
CPU işi:       Task.Run(() => SyncWork()) ile thread pool'a taşı
Sync library:  Wrapper async metod yaz — içinde Task.Run kullan
```

**Tehlike işaretleri:**
- Async metodun içinde `Thread.Sleep(...)` çağrısı
- `File.ReadAllText` yerine `File.ReadAllTextAsync` kullanılmamış
- Async zincirinin ortasında `void` döndüren sync servis metodu
