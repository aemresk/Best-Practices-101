# 09 — Soft Delete Pattern

## Ne Öğretir?

Fiziksel silme yerine `IsDeleted` işaretlemesiyle veriyi gizlemenin nasıl yapılacağını, **global query filter** ile bunu tüm sorgulara şeffaf uygulamayı ve silinen kayıtları geri almanın nasıl işlediğini gösterir.

## Neden Önemli?

Fiziksel silme geri alınamaz:
- Yanlışlıkla silinen veri kurtarılamaz
- Referans veren tablolar bozulabilir (FK ihlali veya orphan kayıt)
- Denetim izi (audit trail) yok — kim ne zaman sildi bilinmiyor
- Raporlama geçmişe erişemez

## Before — Yaygın Hatalar

| Problem | Açıklama |
|---------|----------|
| `db.Products.Remove(p)` | Fiziksel DELETE — veri kalıcı gitti |
| Audit trail yok | Silme zamanı ve sebebi kayıt altına alınmıyor |
| Geri alma yolu yok | Yanlış silme = veri kaybı |

## After — Doğru Yaklaşım

| Çözüm | Açıklama |
|-------|----------|
| `ISoftDeletable` interface | `IsDeleted`, `DeletedAt` — tüm silinebilir entity'ler için sözleşme |
| `HasQueryFilter(x => !x.IsDeleted)` | Global filter: tüm sorgulara otomatik eklen, endpoint kodu değişmez |
| `SaveChangesAsync` override | `Remove()` çağrısı `DELETE` yerine `UPDATE IsDeleted=1` çalıştırır |
| `IgnoreQueryFilters()` | Admin endpoint'leri için global filter'ı geçici bypass et |
| `/products/{id}/restore` | Soft delete geri alınabilir — `IsDeleted=false` |

## Mimarinin Özü

```
db.Products.Remove(p)
    ↓ AppDbContext.SaveChangesAsync()
    ↓ EntityState.Deleted → EntityState.Modified
    ↓ IsDeleted = true, DeletedAt = now
    ↓ UPDATE Products SET IsDeleted=1, DeletedAt=... WHERE Id=?
```

Endpoint kodu hiç değişmedi — davranış DbContext katmanında değişti.

## Nasıl Çalıştırılır?

```bash
# Before
cd Before && dotnet run

# After
cd After && dotnet run
```

```bash
# Aktif ürünler (silinen gözükmez)
GET http://localhost:5000/products

# Sil (soft delete)
DELETE http://localhost:5000/products/1

# Silinen ürünleri listele (admin)
GET http://localhost:5000/products/deleted

# Geri al
POST http://localhost:5000/products/1/restore
```
