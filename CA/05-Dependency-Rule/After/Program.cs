using Domain;
using Infrastructure;

// ✅ Composition Root: hangi implementasyonun kullanılacağına burada karar verilir.
//    Test ortamında: new NoOpEmailSender() — gerçek e-posta gönderilmez.
//    Prod ortamında: new SmtpEmailSender()

var orderService = new OrderService(
    emailSender: new SmtpEmailSender(),
    auditLogger: new FileAuditLogger()
);

orderService.Place("Mehmet Kaya", 2500m);

Console.WriteLine("\n--- Test Ortamı ---");
var testService = new OrderService(
    emailSender: new NoOpEmailSender(),   // ✅ Domain değişmedi, sadece implementasyon değişti
    auditLogger: new FileAuditLogger()
);
testService.Place("Test Kullanıcı", 100m);
