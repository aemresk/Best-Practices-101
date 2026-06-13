// ✅ Rich Domain Model:
//    BankAccount kendi iş kurallarını bilir ve korur.
//    Dışarıdan yalnızca anlamlı işlemler çağrılabilir.

using Domain;

var account = BankAccount.Open(owner: "Mehmet Kaya", initialBalance: 1000m, overdraftLimit: 500m);

account.Deposit(250m);
account.Withdraw(800m);
account.PrintStatement();

try { account.Withdraw(5000m); }
catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
