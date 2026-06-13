namespace Application;

// ✅ DTO: yalnızca istemciye gösterilecek alanlar
//    Hassas alanlar (PasswordHash, FailedLoginCount) burada yok
public record UserProfileDto(
    int    Id,
    string Email,
    string FullName,
    string Role,
    string LastLoginAt   // ✅ Formatlı string — DateTime değil
);

// ✅ Admin için farklı projeksiyon — ihtiyaca göre genişletilebilir
public record UserAdminDto(
    int    Id,
    string Email,
    string FullName,
    string Role,
    bool   IsActive,
    int    FailedLoginCount,
    string LastLoginAt
);
