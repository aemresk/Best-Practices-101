# 03 — Liskov Substitution Principle (LSP)

> **"Alt sınıflar, üst sınıflarının yerine geçebilmelidir."**

Bir alt sınıf, üst sınıfın davranış sözleşmesini bozmadan kullanılabilmelidir.

## Senaryolar

| # | Senaryo | Before İhlali | After Çözümü |
|---|---------|--------------|--------------|
| a | Bird / Penguin | `Penguin.Fly()` → NotSupportedException fırlatır | `IFlyable` ayrı interface — Penguin onu implement etmez |
| b | ReadOnlyRepository | `ReadOnlyRepo.Add()` → NotImplementedException fırlatır | `IReadRepository` / `IWriteRepository` — bağımsız sözleşmeler |

## Tehlike İşaretleri

- Alt sınıfın bir metodunda `throw new NotSupportedException()`
- `if (animal is Penguin) skip` gibi tip kontrolleri
- Override edilmiş metodun içinin boş bırakılması
