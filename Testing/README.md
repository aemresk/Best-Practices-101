# Testing Best Practices

Test kodu da production kod kadar bakım ister.
Bu seri; kırılgan, okunması zor ve niyeti belirsiz testleri daha anlaşılır, davranış odaklı ve sürdürülebilir hale getirir.

## İçerik

| No | Konu | Ana fikir |
|----|------|-----------|
| 01 | [Test Structure](01-Test-Structure/) | Test ismi, Arrange-Act-Assert ve açık assertion |
| 02 | [Test Data Builders](02-Test-Data-Builders/) | Gürültülü test verisini builder ile sadeleştirme |

## Kural

```text
İyi test, hata verdiğinde neyin bozulduğunu hızlıca anlatır.
Testin amacı setup detaylarının arasında kaybolmamalıdır.
```

**Tehlike işaretleri:**
- `Test1`, `ShouldWork`, `CheckOrder` gibi belirsiz test isimleri
- Tek test içinde birden fazla davranış doğrulama
- Kopyala-yapıştır object initializer blokları
- `Assert.True(...)` içinde niyeti saklayan karmaşık koşullar
