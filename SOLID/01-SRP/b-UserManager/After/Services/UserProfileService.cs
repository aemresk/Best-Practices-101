// ✅ Tek sorumluluk: kullanıcı profil bilgilerini güncellemek
// Değişim sebebi: profil alanları genişlediğinde veya avatar depolama değiştiğinde
public class UserProfileService
{
    private readonly UserStore _store;

    public UserProfileService(UserStore store) => _store = store;

    public User UpdateProfile(int userId, UpdateProfileRequest request)
    {
        var user = _store.FindById(userId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı");
        user.Bio       = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        Console.WriteLine($"[LOG] Profil güncellendi: id={userId}");
        return user;
    }
}
