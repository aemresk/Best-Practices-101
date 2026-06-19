# 02 — Test Data Builders

> Test verisi, testin amacını gölgelememeli.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [Order Eligibility](Before/) | Her testte uzun ve tekrar eden object initializer |

## Fark

| | Before | After |
|--|--------|-------|
| Okunabilirlik | Testin amacı veri kalabalığında kaybolur | Sadece senaryo için önemli alanlar görünür |
| Bakım | Model değişince çok test kırılır | Varsayılanlar builder içinde güncellenir |
| Tekrar | Aynı setup blokları kopyalanır | `OrderBuilder.ValidOrder()` tekrar kullanılır |
| Niyet | Hangi alanın önemli olduğu belirsiz | `WithItemCount(0)` gibi davranış odaklı kurulum |

## Kural

```text
Testte sadece senaryo için önemli veriyi göster.
Geçerli varsayılanları builder içinde sakla.
```

**Tehlike işaretleri:**
- Her testte 10+ satırlık nesne oluşturma
- Testlerin çoğu aynı değerleri tekrar ediyor
- Model alanı eklenince davranışla ilgisiz testler kırılıyor
