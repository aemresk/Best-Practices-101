// ✅ Banka havalesi mantığı izole
public class BankTransferPaymentStrategy : IPaymentStrategy
{
    public string Method => "BankTransfer";

    public string Execute(decimal amount)
    {
        Console.WriteLine($"[Bank] IBAN doğrulanıyor... {amount:C2} havale ediliyor");
        Console.WriteLine("[Bank] Havale 1-3 iş günü içinde tamamlanacak");
        return "Pending";
    }
}
