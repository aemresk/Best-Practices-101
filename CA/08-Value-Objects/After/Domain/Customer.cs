namespace Domain;

public class Customer
{
    public int         Id          { get; private set; }
    public string      FullName    { get; private set; }
    public Email       Email       { get; private set; }       // ✅ Value Object
    public PhoneNumber PhoneNumber { get; private set; }       // ✅ Value Object
    public Money       Balance     { get; private set; }       // ✅ Para birimi bilgisi ile birlikte
    public Address     Address     { get; private set; }       // ✅ Adres bir bütün

    private Customer() { FullName = ""; Email = null!; PhoneNumber = null!; Balance = null!; Address = null!; }

    public static Customer Create(int id, string fullName, Email email,
                                  PhoneNumber phone, Money balance, Address address)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Ad soyad zorunlu");
        return new Customer
        {
            Id          = id,
            FullName    = fullName.Trim(),
            Email       = email,
            PhoneNumber = phone,
            Balance     = balance,
            Address     = address
        };
    }

    public void Deposit(Money amount)
    {
        Balance = Balance.Add(amount);  // ✅ Para birimi uyumsuzluğu burada yakalanır
    }
}
