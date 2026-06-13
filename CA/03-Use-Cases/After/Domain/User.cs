namespace Domain;

public class User
{
    public int    Id           { get; private set; }
    public string Email        { get; private set; }
    public string FullName     { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt  { get; private set; }

    private User() { Email = ""; FullName = ""; PasswordHash = ""; }

    public static User Register(int id, string email, string fullName, string passwordHash)
    {
        return new User
        {
            Id           = id,
            Email        = email.ToLowerInvariant(),
            FullName     = fullName,
            PasswordHash = passwordHash,
            CreatedAt    = DateTime.UtcNow
        };
    }
}
