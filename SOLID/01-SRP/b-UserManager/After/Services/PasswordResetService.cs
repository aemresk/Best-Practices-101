// ✅ Tek sorumluluk: şifre sıfırlama akışı
// Değişim sebebi: token süresi veya sıfırlama kanalı değiştiğinde
public class PasswordResetService
{
    private readonly UserStore _store;
    private readonly PasswordHasher _hasher;
    private readonly IUserEmailSender _email;

    public PasswordResetService(UserStore store, PasswordHasher hasher, IUserEmailSender email)
    {
        _store  = store;
        _hasher = hasher;
        _email  = email;
    }

    public void RequestReset(ResetPasswordRequest request)
    {
        var user = _store.FindByEmail(request.Email)
            ?? throw new KeyNotFoundException("E-posta bulunamadı");
        user.ResetToken       = Guid.NewGuid().ToString("N");
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        _email.SendPasswordResetLink(user, user.ResetToken);
    }

    public void ConfirmReset(ConfirmResetRequest request)
    {
        var user = _store.FindByResetToken(request.Token)
            ?? throw new KeyNotFoundException("Geçersiz token");
        if (user.ResetTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Token süresi dolmuş");
        user.PasswordHash     = _hasher.Hash(request.NewPassword);
        user.ResetToken       = null;
        user.ResetTokenExpiry = null;
        Console.WriteLine($"[LOG] Şifre sıfırlandı: {user.Email}");
    }
}
