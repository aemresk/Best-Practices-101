# 03 — ConfigureAwait

> Library ve infrastructure kodu her `await`'e `.ConfigureAwait(false)` eklemeli; uygulama katmanı eklememeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Product Repository](Before/) | Repository'de `ConfigureAwait(false)` yok — gereksiz context switch |

## Ne Yapar?

`await task` → tamamlandığında **aynı SynchronizationContext'e** geri döner (UI thread, ASP.NET request context…)  
`await task.ConfigureAwait(false)` → tamamlandığında **herhangi bir thread pool thread'inde** devam eder

## Kural

| Katman | ConfigureAwait(false)? | Neden |
|--------|----------------------|-------|
| Library / Infrastructure / Repository | **Evet** | Context bağımlılığı yok, deadlock kapısını kapat |
| UI kodu (WPF/WinForms event handler) | **Hayır** | Sonrasında UI elementi güncelleniyor olabilir |
| ASP.NET Core controller / middleware | **Tercihe bağlı** | SyncContext yok, deadlock riski yok; ama yine de önerilir |

## Fark

| | Before | After |
|--|--------|-------|
| Context switch | Her `await` sonrası gereksiz geri dönüş | Yok — thread pool'da devam |
| Deadlock riski | `.Result` ile çağıranda deadlock | Yok |
| Performans | Ekstra scheduler geçişi | Minimum overhead |

**Tehlike işaretleri:**
- NuGet paketi / shared library kodunda `ConfigureAwait(false)` eksik
- Repository veya service sınıfında `HttpContext`, `SynchronizationContext` erişimi yok ama `ConfigureAwait` de yok
