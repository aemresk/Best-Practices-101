using Domain;

namespace Infrastructure;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _store = new();
    private int _sequence = 1;

    public void Save(User user)                    => _store.Add(user);
    public bool ExistsByEmail(string email)        => _store.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    public int  NextId()                           => _sequence++;
}

public class Sha256PasswordHasher : IPasswordHasher
{
    private const string Salt = "sabit_tuz_123";

    public string Hash(string password) =>
        Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password + Salt)));
}

public class ConsoleEmailService : IEmailService
{
    public void SendWelcome(string email, string name)
        => Console.WriteLine($"[EMAIL] Hoş geldiniz e-postası → {email} ({name})");
}
