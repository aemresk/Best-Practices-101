// ✅ Tek sorumluluk: şifre hashleme
// Algoritma değişirse (bcrypt, Argon2...) yalnızca bu sınıf açılır
public class PasswordHasher
{
    public string Hash(string password) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));

    public bool Verify(string password, string hash) => Hash(password) == hash;
}
