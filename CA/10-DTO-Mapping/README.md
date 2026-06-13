# 10 — DTO Mapping (Application Layer DTOs)

> Domain entity'leri asla doğrudan dışarıya döndürülmemeli; her use case kendi projeksiyon DTO'sunu kullanmalı.

## Senaryo

| | Senaryo | Anti-pattern |
|--|---------|--------------|
| — | [User Profile](Before/) | `GetById` doğrudan `User` entity döndürüyor — `PasswordHash` API'de görünüyor |

## Fark

| | Before | After |
|--|--------|-------|
| API response | `User` entity — `PasswordHash`, `FailedLoginCount` dahil | `UserProfileDto` — sadece güvenli alanlar |
| Kontrat bağımlılığı | Domain entity değişince API kontratı kırılır | DTO bağımsız gelişir |
| Projeksiyon | Tek model: herkese aynı veri | `UserProfileDto` (public) + `UserAdminDto` (admin) |
| Mapping | Yok — kopyalama dağınık | `UserMapper.ToProfileDto()` — tek yer |

## Kural

```
Application katmanı DTO döndürür, domain entity döndürmez.
Her use case (public/admin/mobile) kendi DTO projeksiyonuna sahip olabilir.
```

**Tehlike işaretleri:**
- Servis metodu dönüş tipi domain entity sınıfı
- `[JsonIgnore]` attribute'ları domain entity üzerinde biriküyor
- API şeması domain entity şemasıyla birebir örtüşüyor
