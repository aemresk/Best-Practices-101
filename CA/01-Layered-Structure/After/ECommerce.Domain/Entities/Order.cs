namespace ECommerce.Domain.Entities;

public class Order
{
    public int     Id            { get; private set; }
    public string  CustomerName  { get; private set; }
    public string  CustomerEmail { get; private set; }
    public decimal Amount        { get; private set; }
    public decimal Discount      { get; private set; }
    public decimal FinalAmount   => Amount * (1 - Discount);
    public OrderStatus Status    { get; private set; }

    private Order() { CustomerName = ""; CustomerEmail = ""; }

    // ✅ İş kuralları entity içinde — Create dışından geçersiz Order oluşturulamaz
    public static Order Create(int id, string customerName, string customerEmail,
                               decimal amount, decimal discount)
    {
        if (string.IsNullOrWhiteSpace(customerName))  throw new ArgumentException("Ad zorunlu");
        if (!customerEmail.Contains('@'))              throw new ArgumentException("Geçersiz e-posta");
        if (amount <= 0)                               throw new ArgumentException("Tutar sıfırdan büyük olmalı");
        if (discount is < 0 or >= 1)                  throw new ArgumentException("İndirim 0-1 arasında olmalı");

        return new Order
        {
            Id            = id,
            CustomerName  = customerName,
            CustomerEmail = customerEmail,
            Amount        = amount,
            Discount      = discount,
            Status        = OrderStatus.Pending
        };
    }

    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Cancel()  => Status = OrderStatus.Cancelled;
}

public enum OrderStatus { Pending, Confirmed, Cancelled }
