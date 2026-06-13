using Domain;

namespace Application;

// ✅ Mapper: domain → DTO dönüşümü tek yerde, her iki taraf bağımsız gelişebilir
public static class UserMapper
{
    public static UserProfileDto ToProfileDto(User user) =>
        new(
            Id:          user.Id,
            Email:       user.Email,
            FullName:    user.FullName,
            Role:        user.Role,
            LastLoginAt: user.LastLoginAt.ToString("dd.MM.yyyy HH:mm")
        );

    public static UserAdminDto ToAdminDto(User user) =>
        new(
            Id:               user.Id,
            Email:            user.Email,
            FullName:         user.FullName,
            Role:             user.Role,
            IsActive:         user.IsActive,
            FailedLoginCount: user.FailedLoginCount,
            LastLoginAt:      user.LastLoginAt.ToString("dd.MM.yyyy HH:mm")
        );
}
