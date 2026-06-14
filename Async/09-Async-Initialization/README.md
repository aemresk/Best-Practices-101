# 09 — Async Initialization

> Constructor async olamaz — async init için static factory, `Lazy<Task<T>>` veya `InitializeAsync` pattern'lerinden biri kullanılmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [UserRepository](Before/) | Constructor'da `.GetAwaiter().GetResult()` — deadlock riski |

## 3 Kabul Görmüş Pattern

| Pattern | Ne Zaman |
|---------|----------|
| `static async Task<T> CreateAsync()` | Nesne her oluşturulduğunda async init gerekiyor |
| `Lazy<Task<T>>` | Singleton, yalnızca bir kez init, thread-safe |
| `InitializeAsync()` | DI container, `IHostedService`, host startup akışı |

## Fark

| | Before | After |
|--|--------|-------|
| Constructor | `.GetResult()` — blocking, deadlock | `private` — dışarıdan `new` yok |
| Oluşturma | `new UserRepository()` — sync, bloke | `await UserRepository.CreateAsync()` |
| Init | Her zaman çalışır, blocking | Factory / Lazy / InitializeAsync seçenekleri |

## Kural

```
Constructor'da async iş yapma.
Async iş gerektiren nesneler için:
  → static async factory  (new yerine)
  → Lazy<Task<T>>         (singleton + lazy)
  → InitializeAsync()     (DI + host startup)
```

**Tehlike işaretleri:**
- Constructor'da `.Result`, `.Wait()` veya `.GetAwaiter().GetResult()` çağrısı
- `_data = LoadAsync().Result` şeklinde field başlatma
- Constructor'dan `Task.Run(() => InitAsync()).Wait()` çağrısı
