# 01 — Test Structure (AAA + Naming)

> Testler senaryoyu, aksiyonu ve beklenen sonucu açıkça göstermeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Discount](Before/) | Belirsiz test ismi, karışık setup, genel assertion |

## Fark

| | Before | After |
|--|--------|-------|
| İsim | `Test1`, `ShouldWork` | `CalculateTotal_WhenCustomerIsPremium_ShouldApplyDiscount` |
| Akış | Setup, aksiyon ve doğrulama iç içe | Arrange, Act, Assert net |
| Assertion | `AssertTrue(total < 100)` | Beklenen değer açık: `90m` |
| Hata mesajı | Neyin bozulduğu belirsiz | Davranış doğrudan görünüyor |

## Kural

```text
Test adı: MethodName_WhenCondition_ShouldExpectedResult
Test gövdesi: Arrange -> Act -> Assert
Assertion: niyeti saklamayan, beklenen değeri açık gösteren ifade
```

**Tehlike işaretleri:**
- Test ismi davranışı anlatmıyor
- Test içinde birden fazla bağımsız senaryo var
- Assertion, testin gerçek beklentisini saklıyor
