namespace Domain;

// ✅ Domain entity: tüm alanları içerir — bunlar asla doğrudan dışarıya çıkmaz
public class User
{
    public int      Id               { get; private set; }
    public string   Email            { get; private set; }
    public string   FullName         { get; private set; }
    public string   PasswordHash     { get; private set; }  // hassas — DTO'ya gitmez
    public string   Role             { get; private set; }
    public int      FailedLoginCount { get; private set; }  // iç durum — DTO'ya gitmez
    public DateTime LastLoginAt      { get; private set; }
    public bool     IsActive         { get; private set; }

    private User() { Email = ""; FullName = ""; PasswordHash = ""; Role = ""; }

    public static User Create(int id, string email, string fullName,
                              string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(email))    throw new ArgumentException("E-posta zorunlu");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Ad soyad zorunlu");

        return new User
        {
            Id               = id,
            Email            = email.ToLowerInvariant(),
            FullName         = fullName,
            PasswordHash     = passwordHash,
            Role             = role,
            FailedLoginCount = 0,
            LastLoginAt      = DateTime.UtcNow,
            IsActive         = true
        };
    }

    public void RecordFailedLogin() => FailedLoginCount++;
    public void RecordSuccessfulLogin() { FailedLoginCount = 0; LastLoginAt = DateTime.UtcNow; }
}
