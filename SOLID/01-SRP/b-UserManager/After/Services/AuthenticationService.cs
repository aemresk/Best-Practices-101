// ✅ Tek sorumluluk: kimlik doğrulama ve token üretme
// Değişim sebebi: JWT'ye geçiş veya MFA eklenmesi
public class AuthenticationService
{
    private readonly UserStore _store;
    private readonly PasswordHasher _hasher;

    public AuthenticationService(UserStore store, PasswordHasher hasher)
    {
        _store  = store;
        _hasher = hasher;
    }

    public string Login(LoginRequest request)
    {
        var user = _store.FindByEmail(request.Email)
            ?? throw new UnauthorizedAccessException("Kullanıcı bulunamadı");
        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Şifre hatalı");

        // Gerçek projede JWT token döner; örneği sade tutmak için Base64 GUID
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        Console.WriteLine($"[LOG] Giriş başarılı: {user.Email}");
        return token;
    }
}
