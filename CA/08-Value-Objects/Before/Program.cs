// ❌ Primitive Obsession Anti-pattern:
//    Domain kavramları ham tipler (string, decimal) olarak temsil ediliyor.
//    Validasyon her yerde tekrar yazılmak zorunda.
//    Yanlış atama derleyici tarafından yakalanamaz (para birimleri karışabilir).

var customer = new Customer
{
    Id          = 1,
    FullName    = "Ahmet Yılmaz",
    Email       = "ahmet@ornek.com",   // ❌ string — geçersiz e-posta atanabilir
    PhoneNumber = "+905551234567",      // ❌ string — format kontrolü her yerde tekrar
    Balance     = 1500.75m,            // ❌ decimal — hangi para birimi? bilinmiyor
    Currency    = "TRY",               // ❌ ayrı string — Balance ile tutarsız olabilir
    Street      = "Atatürk Cad. No:5",
    City        = "İstanbul",          // ❌ adres parçaları ayrı string — bütün olarak taşınamaz
    ZipCode     = "34000"
};

// ❌ Validasyon her yerde tekrar
if (!customer.Email.Contains('@')) throw new Exception("Geçersiz e-posta");
if (customer.Balance < 0)          throw new Exception("Bakiye negatif olamaz");
if (customer.ZipCode.Length != 5)  throw new Exception("Geçersiz posta kodu");

// ❌ Para birimi karışabilir — derleyici yakalamaz
decimal usdAmount = 100m;
decimal tryAmount = 500m;
decimal total     = usdAmount + tryAmount;  // ❌ Anlamsız toplama ama derlenir

Console.WriteLine($"Müşteri: {customer.FullName} | {customer.Balance} {customer.Currency}");
Console.WriteLine($"Adres: {customer.Street}, {customer.City} {customer.ZipCode}");

class Customer
{
    public int     Id          { get; set; }
    public string  FullName    { get; set; } = "";
    public string  Email       { get; set; } = "";     // ❌ primitive
    public string  PhoneNumber { get; set; } = "";     // ❌ primitive
    public decimal Balance     { get; set; }           // ❌ primitive (para birimi bilgisi yok)
    public string  Currency    { get; set; } = "";     // ❌ ayrı primitive
    public string  Street      { get; set; } = "";     // ❌ adres parçaları dağınık
    public string  City        { get; set; } = "";
    public string  ZipCode     { get; set; } = "";
}
