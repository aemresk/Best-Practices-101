// ❌ SRP İHLALİ: Tek bir sınıf 5 farklı sorumluluğu üstleniyor
// Bu sınıfı değiştirmek için 5 farklı neden var:
//   1. Kayıt kuralları değişirse
//   2. Şifre hash algoritması değişirse
//   3. Oturum açma / token mantığı değişirse
//   4. Profil alanları değişirse
//   5. Şifre sıfırlama akışı değişirse
public class UserManager
{
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    // ❌ SORUMLULUK 1: Kullanıcı kaydı
    public User Register(RegisterRequest request)
    {
        // ❌ Validasyon ve kayıt iç içe geçmiş
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Kullanıcı adı zorunlu");
        if (!request.Email.Contains('@'))
            throw new ArgumentException("Geçersiz e-posta");
        if (request.Password.Length < 8)
            throw new ArgumentException("Şifre en az 8 karakter olmalı");
        if (_users.Any(u => u.Email == request.Email))
            throw new InvalidOperationException("Bu e-posta zaten kayıtlı");

        // ❌ SORUMLULUK 2: Şifre hashleme — şifreleme değişirse bu sınıf açılır
        var hash = HashPassword(request.Password);

        var user = new User
        {
            Id           = _nextId++,
            Username     = request.Username,
            Email        = request.Email,
            PasswordHash = hash
        };
        _users.Add(user);

        // ❌ SORUMLULUK 3: E-posta gönderme — e-posta sistemi değişirse bu sınıf açılır
        Console.WriteLine($"[EMAIL] Hoş geldiniz {user.Username}! Hesabınız oluşturuldu.");
        Console.WriteLine($"[LOG] Yeni kullanıcı: {user.Email} | {DateTime.UtcNow:O}");

        return user;
    }

    // ❌ SORUMLULUK 1 (devamı): Oturum açma
    public string Login(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Email == request.Email)
            ?? throw new UnauthorizedAccessException("Kullanıcı bulunamadı");
        if (!VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Şifre hatalı");

        // ❌ SORUMLULUK 4: Token üretme — JWT'ye geçilince bu sınıf açılır
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        Console.WriteLine($"[LOG] Giriş başarılı: {user.Email} | token={token}");
        return token;
    }

    // ❌ SORUMLULUK 5: Profil güncelleme
    public User UpdateProfile(int userId, UpdateProfileRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı");
        user.Bio       = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        Console.WriteLine($"[LOG] Profil güncellendi: id={userId}");
        return user;
    }

    // ❌ SORUMLULUK 5 (devamı): Şifre sıfırlama — akış değişirse bu sınıf açılır
    public void RequestPasswordReset(ResetPasswordRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Email == request.Email)
            ?? throw new KeyNotFoundException("E-posta bulunamadı");
        user.ResetToken  = Guid.NewGuid().ToString("N");
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        Console.WriteLine($"[EMAIL] Şifre sıfırlama linki: /reset?token={user.ResetToken}");
    }

    public void ConfirmPasswordReset(ConfirmResetRequest request)
    {
        var user = _users.FirstOrDefault(u => u.ResetToken == request.Token)
            ?? throw new KeyNotFoundException("Geçersiz token");
        if (user.ResetTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Token süresi dolmuş");
        user.PasswordHash = HashPassword(request.NewPassword);
        user.ResetToken   = null;
        user.ResetTokenExpiry = null;
        Console.WriteLine($"[LOG] Şifre sıfırlandı: {user.Email}");
    }

    public List<User> GetAll() => _users;

    // ❌ Hash mantığı sınıfın içine gömülü — algoritma değişince burası açılır
    private static string HashPassword(string password) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));

    private static bool VerifyPassword(string password, string hash) =>
        HashPassword(password) == hash;
}
