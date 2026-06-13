using Domain;

var customer = Customer.Create(
    id:       1,
    fullName: "Ahmet Yılmaz",
    email:    new Email("ahmet@ornek.com"),
    phone:    new PhoneNumber("+905551234567"),
    balance:  new Money(1500.75m, "TRY"),
    address:  new Address("Atatürk Cad. No:5", "İstanbul", "34000")
);

Console.WriteLine($"Müşteri : {customer.FullName}");
Console.WriteLine($"E-posta : {customer.Email}");
Console.WriteLine($"Telefon : {customer.PhoneNumber}");
Console.WriteLine($"Bakiye  : {customer.Balance}");
Console.WriteLine($"Adres   : {customer.Address}");

customer.Deposit(new Money(500m, "TRY"));
Console.WriteLine($"\nPara yatırıldı. Yeni bakiye: {customer.Balance}");

// ✅ Value Object eşitliği: değer bazlıdır
var email1 = new Email("TEST@Ornek.COM");
var email2 = new Email("test@ornek.com");
Console.WriteLine($"\nE-posta eşitliği: {email1 == email2}");  // True

// ✅ Geçersiz değer denemeye çalışalım
try { var _ = new Email("gecersiz-email"); }
catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }

// ✅ Para birimi uyuşmazlığı
try
{
    var try1 = new Money(1000m, "TRY");
    var usd1 = new Money(50m,   "USD");
    try1.Add(usd1);  // ❌ TRY + USD — çalışma zamanında yakalanır
}
catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
