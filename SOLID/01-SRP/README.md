# 01 — Single Responsibility Principle (SRP)

> Bir sınıfın değişmesi için yalnızca tek bir nedeni olmalıdır.

## Senaryolar

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| a | [OrderService](a-OrderService/) | Sipariş oluşturma + indirim + e-posta + loglama tek sınıfta |
| b | [UserManager](b-UserManager/) | Kayıt + giriş + profil güncelleme + şifre sıfırlama tek sınıfta |

## Kural

```
Her sınıfın yalnızca bir değişim sebebi olmalı.
"Bu sınıf neden değişir?" sorusunun tek bir cevabı olmalı.
```

**Tehlike işaretleri:**
- Sınıf adı `Manager`, `Helper`, `Util`, `Processor` içeriyorsa
- Sınıf birden fazla kavramla ilgili field/method barındırıyorsa
- Bir özellik değiştiğinde ilgisiz alanlar da açılıp inceleniyorsa
