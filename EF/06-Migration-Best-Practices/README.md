# 06 — Migration Best Practices

## Ne Öğretir?

EF Core migration sistemini nasıl doğru kullanacağını, `EnsureCreated` tuzağını, migration isimlendirme kurallarını ve veri migration'larını gösterir.

## Neden Önemli?

`EnsureCreated()` geliştirme sürecinde işe yarar görünür, ama production'da:
- Şema değişikliklerini takip edemezsiniz
- Var olan bir veritabanına yeni kolon/index ekleyemezsiniz
- Rollback yapmanın yolu yoktur
- Ekip arkadaşları hangi değişikliklerin uygulandığını bilemez

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `EnsureCreated()` | Migration sistemi devre dışı — `__EFMigrationsHistory` oluşmaz |
| Seed kodu Program.cs'te | Şema ve veri sorumluluğu karışıyor |
| Migration yok | Şema değişikliği → DB'yi elle sil veya ALTER TABLE yaz |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `Database.Migrate()` | Bekleyen migration'ları sırayla uygular, geçmişi takip eder |
| Açıklayıcı migration isimleri | `CreateUsersTable`, `AddUniqueIndexOnUserEmail` — ne yapıldığı açık |
| Veri migration'ı ayrı | `SeedAdminUser` — şema değişikliğiyle karışmıyor |
| `Sql()` ile idempotent seed | Migration bir kez çalışır, `__EFMigrationsHistory`'de kayıtlı |

## Migration Dosyaları

```
After/Migrations/
├── 20240601000000_CreateUsersTable.cs          ← şema: tablo oluştur
├── 20240601000001_AddUniqueIndexOnUserEmail.cs ← şema: index ekle
├── 20240601000002_SeedAdminUser.cs             ← veri: admin kullanıcı ekle
└── AppDbContextModelSnapshot.cs               ← EF araçları tarafından üretilir
```

> `AppDbContextModelSnapshot.cs` elle yazılmaz — `dotnet ef migrations add` her çalıştırıldığında otomatik güncellenir.

## Temel CLI Komutları

```bash
# Yeni migration oluştur
dotnet ef migrations add AddPhoneNumberToUsers

# Bekleyen migration'ları uygula
dotnet ef database update

# Production için idempotent SQL script üret
dotnet ef migrations script --idempotent -o migrate.sql

# Son migration'ı geri al (henüz uygulanmadıysa)
dotnet ef migrations remove
```

## İsimlendirme Kuralı

```
✅  CreateUsersTable
✅  AddUniqueIndexOnUserEmail
✅  RenameProductNameToTitle
✅  SeedAdminUser
✅  AddPhoneNumberToUsers

❌  Initial
❌  Migration1
❌  Update
❌  Fix
```

## Nasıl Çalıştırılır?

```bash
# Before
cd Before
dotnet run

# After
cd After
dotnet run
# → Migration'lar otomatik uygulanır, admin kullanıcı eklenir
```

```bash
GET  http://localhost:5000/users
POST http://localhost:5000/users
Content-Type: application/json
{ "name": "Ali", "email": "ali@example.com" }
```
