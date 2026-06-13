// ✅ Tek sorumluluk: kullanıcıya e-posta göndermek
public interface IUserEmailSender
{
    void SendWelcome(User user);
    void SendPasswordResetLink(User user, string token);
}

public class ConsoleUserEmailSender : IUserEmailSender
{
    public void SendWelcome(User user) =>
        Console.WriteLine($"[EMAIL] Hoş geldiniz {user.Username}! Hesabınız oluşturuldu.");

    public void SendPasswordResetLink(User user, string token) =>
        Console.WriteLine($"[EMAIL] {user.Email} → Şifre sıfırlama linki: /reset?token={token}");
}
