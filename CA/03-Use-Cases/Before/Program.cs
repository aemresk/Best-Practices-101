// ❌ Use Case Anti-pattern:
//    İş mantığı "endpoint / controller" seviyesinde, tek büyük metodda.
//    Validasyon + şifreleme + persistance + bildirim + loglama iç içe.
//    Test etmek, değiştirmek ve yeniden kullanmak neredeyse imkânsız.

var users = new List<(string Email, string Name, string Hash)>();

RegisterUser("ali@ornek.com", "Güçlü!Şifre123", "Ali Veli");

try { RegisterUser("", "abc", ""); }
catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }

void RegisterUser(string email, string password, string name)
{
    // ❌ Validasyon inline — her endpoint tekrar yazıyor
    if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        throw new ArgumentException("Geçerli e-posta zorunlu");
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Ad soyad zorunlu");
    if (password.Length < 8)
        throw new ArgumentException("Şifre en az 8 karakter olmalı");
    if (!password.Any(char.IsUpper) || !password.Any(char.IsDigit))
        throw new ArgumentException("Şifre büyük harf ve rakam içermeli");

    // ❌ Şifreleme inline — hangi algoritma olduğu bilinmiyor, değiştirmek zor
    var salt         = "sabit_tuz_123";
    var passwordHash = Convert.ToHexString(
        System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password + salt)));

    // ❌ Benzersizlik kontrolü inline — DB değişirse bu satır da değişmeli
    if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        throw new InvalidOperationException("Bu e-posta zaten kayıtlı");

    // ❌ Kayıt inline — repository soyutlaması yok
    users.Add((email, name, passwordHash));
    Console.WriteLine($"[DB] Kullanıcı kaydedildi: {email}");

    // ❌ E-posta inline — SMTP detayları business logic içinde
    Console.WriteLine($"[EMAIL] Hoş geldiniz e-postası: {email}");

    // ❌ Loglama inline
    Console.WriteLine($"[LOG] {DateTime.UtcNow:O} | UserRegistered | {email}");

    Console.WriteLine($"✅ Kayıt başarılı: {name} ({email})");
}
