// ❌ Async/Sync karışımı Anti-pattern:
//    Zincirin ortasında sync metod var — async'in faydası yok olur.
//    Sync metod thread'i bloke eder, async metodun kazanımını siler.
//    "Yarı async" kod en kötü dünyayı sunar: hem blocking hem karmaşık.

Console.WriteLine("Kullanıcı kaydı başlıyor...");
await RegisterUserAsync("ali@ornek.com", "Şifre123!");

async Task RegisterUserAsync(string email, string password)
{
    // ✅ async — doğru
    var isUnique = await CheckEmailUniqueAsync(email);
    if (!isUnique) { Console.WriteLine("E-posta zaten kayıtlı."); return; }

    // ❌ Sync metod zinciri kesiyor — thread burada bloke olur
    var hash = HashPassword(password);          // ❌ sync — CPU işi ama yine de zinciri kirletiyor
    var userId = SaveUserToDatabase(email, hash); // ❌ sync — I/O olursa felaket

    // ✅ async — doğru, ama üstteki sync çağrılar faydayı zaten sildi
    await SendWelcomeEmailAsync(email);

    Console.WriteLine($"✅ Kullanıcı kaydedildi: #{userId}");
}

async Task<bool> CheckEmailUniqueAsync(string email)
{
    await Task.Delay(100); // simüle DB sorgusu
    return true;
}

// ❌ Sync: async versiyonu var ama kullanılmıyor
string HashPassword(string password)
{
    Thread.Sleep(50); // simüle yavaş crypto işlemi — thread bloke!
    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
}

// ❌ Sync I/O — en kritik sorun
int SaveUserToDatabase(string email, string hash)
{
    Thread.Sleep(200); // simüle DB yazma — thread 200ms bloke!
    Console.WriteLine($"[DB] {email} kaydedildi");
    return 42;
}

async Task SendWelcomeEmailAsync(string email)
{
    await Task.Delay(80);
    Console.WriteLine($"[EMAIL] Hoş geldiniz: {email}");
}
