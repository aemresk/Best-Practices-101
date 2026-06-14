# 01 — Async Void

> `async void` yalnızca UI event handler'larında kullanılabilir; her yerde `async Task` tercih edilmeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Processing](Before/) | `async void ProcessOrderAsync` — exception kaybolur |

## Fark

| | Before (`async void`) | After (`async Task`) |
|--|----------------------|----------------------|
| Exception yönetimi | Kaybolur / process çöker | `try/catch` ile yakalanır |
| Await | `await ProcessOrderAsync()` ← derleme hatası | `await ProcessOrderAsync()` ✅ |
| Test | Test edilemez | `Assert.ThrowsAsync<T>` çalışır |
| Tamamlanma takibi | Yok | `await` ile garanti |

## Kural

```
async void  → yalnızca event handler (Button.Click vb.), içinde try/catch zorunlu
async Task  → geri kalan her şey
async Task<T> → değer döndüren her şey
```

**Tehlike işaretleri:**
- `async void` olan herhangi bir metod (event handler dışında)
- `_ = SomeMethodAsync()` ile kasıtlı olarak Task'ın ignore edilmesi
- Fire-and-forget için `async void` yazılmış metodlar
