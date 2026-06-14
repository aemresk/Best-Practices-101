// ✅ Async all the way — zincirin her halkası async.
//    I/O operasyonlarının hepsi async versiyonunu kullanır.
//    CPU-yoğun senkron iş → Task.Run ile thread pool'a taşınır.

Console.WriteLine("Kullanıcı kaydı başlıyor...");
await RegisterUserAsync("ali@ornek.com", "Şifre123!");

async Task RegisterUserAsync(string email, string password)
{
    var isUnique = await CheckEmailUniqueAsync(email);
    if (!isUnique) { Console.WriteLine("E-posta zaten kayıtlı."); return; }

    // ✅ CPU-yoğun senkron iş → Task.Run ile thread pool'a taşı, thread bloke etme
    var hash = await Task.Run(() => HashPassword(password));

    // ✅ I/O işlemleri async versiyonuyla
    var userId = await SaveUserToDatabaseAsync(email, hash);

    await SendWelcomeEmailAsync(email);

    Console.WriteLine($"✅ Kullanıcı kaydedildi: #{userId}");
}

async Task<bool> CheckEmailUniqueAsync(string email)
{
    await Task.Delay(100).ConfigureAwait(false);
    return true;
}

// ✅ CPU işi — senkron kalabilir, Task.Run ile çağrılır
string HashPassword(string password)
{
    Thread.Sleep(50); // simüle crypto — Task.Run içinde çalışır, thread pool'da
    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
}

// ✅ Async versiyonu — I/O thread'i bloke etmez
async Task<int> SaveUserToDatabaseAsync(string email, string hash)
{
    await Task.Delay(200).ConfigureAwait(false); // simüle async DB yazma
    Console.WriteLine($"[DB] {email} kaydedildi");
    return 42;
}

async Task SendWelcomeEmailAsync(string email)
{
    await Task.Delay(80).ConfigureAwait(false);
    Console.WriteLine($"[EMAIL] Hoş geldiniz: {email}");
}
