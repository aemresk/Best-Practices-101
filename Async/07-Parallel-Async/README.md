# 07 — Parallel Async (Task.WhenAll / WhenAny)

> Birbirinden bağımsız async işlemler sırayla değil, aynı anda başlatılmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Dashboard](Before/) | User + Orders + Stats sırayla await — 1000ms |

## Fark

| | Before (sıralı) | After (paralel) |
|--|----------------|-----------------|
| Süre | 300 + 500 + 200 = **~1000ms** | max(300, 500, 200) = **~500ms** |
| Başlama | Her biri önceki bitince | Hepsi aynı anda |
| Kod | 3 ayrı `await` | `Task.WhenAll(...)` |

## Ne Zaman Hangisi?

| | Kullanım |
|--|----------|
| `Task.WhenAll` | Hepsinin tamamlanması gerekiyor |
| `Task.WhenAny` | İlk tamamlanan yeterli (race, timeout) |
| Sıralı `await` | B, A'nın sonucuna **bağımlıysa** |

## Kural

```
Bağımsız işlemler → önce Task'ları al, sonra WhenAll ile bekle
Bağımlı işlemler  → sıralı await (doğru yaklaşım)

// ✅ Doğru paralel:
var t1 = GetUserAsync();
var t2 = GetOrdersAsync();
var (u, o) = await Task.WhenAll(t1, t2);

// ❌ Yanlış — hâlâ sıralı:
var u = await GetUserAsync();
var o = await GetOrdersAsync();
```

**Tehlike işaretleri:**
- `foreach` döngüsünde `await service.GetAsync(id)` — liste için WhenAll daha verimli
- Dashboard/özet sayfasında birden fazla bağımsız veri kaynağı sırayla await ediliyor
