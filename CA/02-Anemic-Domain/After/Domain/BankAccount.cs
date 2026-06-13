namespace Domain;

public class BankAccount
{
    private readonly List<Transaction> _transactions = new();

    public int     Id             { get; private set; }
    public string  Owner         { get; private set; }
    public string  Currency      { get; private set; }
    public decimal Balance       { get; private set; }   // ✅ private set — dışarı yalnızca okunur
    public bool    IsActive      { get; private set; }
    public decimal OverdraftLimit{ get; private set; }

    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    private BankAccount() { Owner = ""; Currency = "TRY"; }

    // ✅ Factory method: nesne her zaman geçerli bir durumda oluşturuluyor
    public static BankAccount Open(string owner, decimal initialBalance, decimal overdraftLimit = 0m)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Hesap sahibi zorunlu");
        if (initialBalance < 0)               throw new ArgumentException("Başlangıç bakiyesi negatif olamaz");
        if (overdraftLimit < 0)               throw new ArgumentException("Overdraft limiti negatif olamaz");

        var account = new BankAccount
        {
            Id             = Random.Shared.Next(1000, 9999),
            Owner          = owner,
            Currency       = "TRY",
            Balance        = initialBalance,
            IsActive       = true,
            OverdraftLimit = overdraftLimit
        };
        account._transactions.Add(new Transaction(TransactionType.Deposit, initialBalance, "Açılış bakiyesi", initialBalance));
        return account;
    }

    // ✅ Davranış entity içinde — kurallar her zaman çalışır, servis unutursa bile
    public void Deposit(decimal amount)
    {
        EnsureActive();
        if (amount <= 0) throw new ArgumentException("Yatırma tutarı sıfırdan büyük olmalı");

        Balance += amount;
        _transactions.Add(new Transaction(TransactionType.Deposit, amount, "Para yatırma", Balance));
        Console.WriteLine($"[YATIRMA] {amount:C2} → Yeni bakiye: {Balance:C2}");
    }

    public void Withdraw(decimal amount)
    {
        EnsureActive();
        if (amount <= 0)                        throw new ArgumentException("Çekim tutarı sıfırdan büyük olmalı");
        if (Balance - amount < -OverdraftLimit) throw new InvalidOperationException("Yetersiz bakiye");

        Balance -= amount;
        _transactions.Add(new Transaction(TransactionType.Withdrawal, amount, "Para çekme", Balance));
        Console.WriteLine($"[ÇEKIM] {amount:C2} → Yeni bakiye: {Balance:C2}");
    }

    public void Deactivate()
    {
        if (!IsActive) throw new InvalidOperationException("Hesap zaten pasif");
        IsActive = false;
    }

    public void PrintStatement()
    {
        Console.WriteLine($"\n--- Hesap Özeti: {Owner} ---");
        Console.WriteLine($"  Bakiye     : {Balance:C2} {Currency}");
        Console.WriteLine($"  Durum      : {(IsActive ? "Aktif" : "Pasif")}");
        Console.WriteLine($"  İşlem sayısı: {_transactions.Count}");
        Console.WriteLine();
    }

    private void EnsureActive()
    {
        if (!IsActive) throw new InvalidOperationException("Hesap aktif değil");
    }
}

public record Transaction(TransactionType Type, decimal Amount, string Description, decimal BalanceAfter);
public enum TransactionType { Deposit, Withdrawal }
