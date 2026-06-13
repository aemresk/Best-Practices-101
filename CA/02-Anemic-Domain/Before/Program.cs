// ❌ Anemic Domain Model Anti-pattern:
//    BankAccount sadece bir "veri torbası" — davranışı yok, kuralları yok.
//    Tüm iş mantığı BankAccountService'te.
//    Sonuç: entity her yerden keyfi şekilde değiştirilebilir.

var account = new BankAccount
{
    Id             = 1,
    Owner          = "Mehmet Kaya",
    Balance        = 1000m,
    IsActive       = true,
    OverdraftLimit = 500m
};

var service = new BankAccountService();
service.Deposit(account, 250m);
service.Withdraw(account, 800m);
service.PrintStatement(account);

try { service.Withdraw(account, 5000m); }
catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }

// ❌ Anemic entity: sadece getter/setter, iş mantığı yok
class BankAccount
{
    public int     Id             { get; set; }
    public string  Owner         { get; set; } = "";
    public decimal Balance       { get; set; }       // ❌ public setter — kim isterse değiştirebilir
    public bool    IsActive      { get; set; }
    public decimal OverdraftLimit{ get; set; }
}

// ❌ İş mantığı entity dışında — SRP ihlali gibi görünse de asıl sorun OOP'un kırılması
class BankAccountService
{
    public void Deposit(BankAccount account, decimal amount)
    {
        if (!account.IsActive) throw new InvalidOperationException("Hesap aktif değil");
        if (amount <= 0)       throw new ArgumentException("Tutar sıfırdan büyük olmalı");
        account.Balance += amount;  // ❌ Servis, entity'nin iç state'ini doğrudan manipüle ediyor
        Console.WriteLine($"[YATIRMA] {amount:C2} → Yeni bakiye: {account.Balance:C2}");
    }

    public void Withdraw(BankAccount account, decimal amount)
    {
        if (!account.IsActive) throw new InvalidOperationException("Hesap aktif değil");
        if (amount <= 0)       throw new ArgumentException("Tutar sıfırdan büyük olmalı");
        if (account.Balance - amount < -account.OverdraftLimit)
            throw new InvalidOperationException("Yetersiz bakiye");
        account.Balance -= amount;  // ❌ Servis, entity'nin iç state'ini doğrudan manipüle ediyor
        Console.WriteLine($"[ÇEKIM] {amount:C2} → Yeni bakiye: {account.Balance:C2}");
    }

    public void PrintStatement(BankAccount account)
    {
        Console.WriteLine($"\n--- Hesap Özeti: {account.Owner} ---");
        Console.WriteLine($"  Bakiye: {account.Balance:C2} | Durum: {(account.IsActive ? "Aktif" : "Pasif")}");
        Console.WriteLine();
    }
}
