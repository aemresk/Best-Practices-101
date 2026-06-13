# 08 — Value Objects

> Domain kavramları primitive tipler değil, anlam taşıyan Value Object'ler olarak temsil edilmeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Customer Profile](Before/) | `Email`, `Money`, `Address` ham `string`/`decimal` olarak temsil ediliyor |

## Fark

| | Before (Primitive Obsession) | After (Value Object) |
|--|------------------------------|----------------------|
| E-posta | `string Email` — geçersiz değer atanabilir | `Email` record — constructor'da validate |
| Para | `decimal Balance` + `string Currency` ayrı | `Money` record — birlikte, tutarsız olamaz |
| TRY+USD | `tryAmount + usdAmount` derlenir | `money1.Add(money2)` — hata fırlatır |
| Adres | 3 ayrı `string` field | `Address` record — bütün olarak taşınır |
| Eşitlik | Referans eşitliği | Değer eşitliği (`email1 == email2 → true`) |

## Kural

```
Value Object: kimliği yoktur, değeriyle eşitlenir, immutable'dır.
Oluşturulduğunda geçerlidir — sonradan geçersiz hale gelemez.
```

**Tehlike işaretleri:**
- Entity'de `string Email`, `string PhoneNumber`, `string Currency` alanları
- Validasyon "burada da yapıyoruz" çünkü "başka yerde unutulabilir"
- İki `decimal` toplanıyor ama para birimi kontrol edilmiyor
