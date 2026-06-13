# 02 — Anemic vs Rich Domain Model

> Domain entity'leri sadece veri taşıyıcı olmamalı; iş kurallarını da içermeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [BankAccount](Before/) | Entity getter/setter, tüm mantık servis sınıfında |

## Fark

| | Before (Anemic) | After (Rich) |
|--|----------------|--------------|
| `Balance` | `public set` — kim isterse değiştirir | `private set` — sadece kendi metodları değiştirir |
| Kural "Yetersiz bakiye" | `BankAccountService.Withdraw` içinde | `BankAccount.Withdraw` içinde |
| İşlem geçmişi | Yok | Entity içinde `List<Transaction>` |
| Oluşturma | `new BankAccount { ... }` — geçersiz durum mümkün | `BankAccount.Open(...)` — her zaman geçerli |

## Kural

```
"Tell, Don't Ask" — entity'ye ne yapacağını söyle,
iç durumunu okuyup dışarıda hesaplama.
```

**Tehlike işaretleri:**
- Entity sınıfının tüm property'leri `public get; set;`
- `XxxService` sınıfının adı entity adıyla birebir eşleşiyor (`OrderService`, `UserService`)
- Entity'nin herhangi bir public metodu yok
