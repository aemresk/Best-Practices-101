namespace Domain;

// ✅ Value Object: kimliği yoktur, değeriyle eşitlenir, değişmezdir (immutable)
//    Validasyon tek bir yerde — oluşturulurken.

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@') || !value.Contains('.'))
            throw new ArgumentException($"Geçersiz e-posta: '{value}'");
        Value = value.ToLowerInvariant().Trim();
    }

    public override string ToString() => Value;
}

public sealed record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (digits.Length < 10 || digits.Length > 13)
            throw new ArgumentException($"Geçersiz telefon numarası: '{value}'");
        Value = value.Trim();
    }

    public override string ToString() => Value;
}

public sealed record Money
{
    public decimal Amount   { get; }
    public string  Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)                              throw new ArgumentException("Tutar negatif olamaz");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException($"Geçersiz para birimi: '{currency}'");
        Amount   = amount;
        Currency = currency.ToUpperInvariant();
    }

    // ✅ Aynı para biriminde toplama — farklı birimler derleme/çalışma hatasıyla engellenir
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Para birimleri uyuşmuyor: {Currency} ≠ {other.Currency}");
        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}

public sealed record Address
{
    public string Street  { get; }
    public string City    { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Sokak zorunlu");
        if (string.IsNullOrWhiteSpace(city))   throw new ArgumentException("Şehir zorunlu");
        if (zipCode.Length != 5 || !zipCode.All(char.IsDigit))
            throw new ArgumentException($"Geçersiz posta kodu: '{zipCode}'");
        Street  = street.Trim();
        City    = city.Trim();
        ZipCode = zipCode;
    }

    public override string ToString() => $"{Street}, {City} {ZipCode}";
}
