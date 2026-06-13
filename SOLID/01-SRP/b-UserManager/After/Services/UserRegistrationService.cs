// ✅ Tek sorumluluk: yeni kullanıcı kaydı
// Değişim sebebi: kayıt kuralları veya adımları değiştiğinde
public class UserRegistrationService
{
    private readonly UserStore _store;
    private readonly PasswordHasher _hasher;
    private readonly IUserEmailSender _email;

    public UserRegistrationService(UserStore store, PasswordHasher hasher, IUserEmailSender email)
    {
        _store  = store;
        _hasher = hasher;
        _email  = email;
    }

    public User Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Kullanıcı adı zorunlu");
        if (!request.Email.Contains('@'))
            throw new ArgumentException("Geçersiz e-posta");
        if (request.Password.Length < 8)
            throw new ArgumentException("Şifre en az 8 karakter olmalı");
        if (_store.FindByEmail(request.Email) is not null)
            throw new InvalidOperationException("Bu e-posta zaten kayıtlı");

        var user = new User
        {
            Username     = request.Username,
            Email        = request.Email,
            PasswordHash = _hasher.Hash(request.Password)
        };
        _store.Add(user);
        _email.SendWelcome(user);
        Console.WriteLine($"[LOG] Yeni kullanıcı: {user.Email} | {DateTime.UtcNow:O}");
        return user;
    }
}
