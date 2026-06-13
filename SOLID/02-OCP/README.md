# 02 — Open/Closed Principle (OCP)

> **"Yazılım varlıkları genişlemeye açık, değişime kapalı olmalıdır."**

Yeni bir davranış eklemek için mevcut kodu değiştirmek yerine yeni bir sınıf eklemek yeterli olmalıdır.

## Senaryolar

| # | Senaryo | Before İhlali | After Çözümü |
|---|---------|--------------|--------------|
| a | PaymentProcessor | `switch/if` ile ödeme tipi kontrolü — yeni tip = kod değişikliği | `IPaymentStrategy` — yeni tip = yeni sınıf |
| b | DiscountCalculator | `if/else if` zincirleri — yeni indirim kuralı = kod değişikliği | `IDiscountRule` listesi — yeni kural = yeni sınıf |

## Tehlike İşaretleri

- `switch (type)` veya `if (type == "X")` pattern'leri
- Yeni senaryo eklemek için mevcut metodu açmak zorunda kalmak
- "Bu metoda bir `else if` daha ekledim" yorumları
