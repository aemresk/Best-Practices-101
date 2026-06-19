using System.Text;

namespace PerfDemo.Demos;

// String operasyonları: birleştirme, karşılaştırma, arama optimizasyonları
internal static class StringOpsDemo
{
    public static void Run()
    {
        DemoRunner.Section("04 — String Optimizasyonları");

        // Her += yeni bir string nesnesi oluşturur (eski + yeni kopyalanır)
        DemoRunner.Compare(
            "50 parça string birleştirme",
            "s += \"parça\"                  [❌ 50 geçici string alloc]",
            () =>
            {
                var s = string.Empty;
                for (int i = 0; i < 50; i++) s += "parça";
            },
            "StringBuilder.Append           [✅ tek buffer, resize ile büyür]",
            () =>
            {
                var sb = new StringBuilder(256);
                for (int i = 0; i < 50; i++) sb.Append("parça");
                _ = sb.ToString();
            },
            iterations: 20_000);

        // Contains(string) kültür kurallarını kontrol eder; Contains(char) doğrudan
        const string sentence = "Performans için doğru araçları kullanmak önemlidir.";
        DemoRunner.Compare(
            "Tek karakter arama",
            "str.Contains(\"r\")             [❌ StringComparison overhead]",
            () => { _ = sentence.Contains("r"); },
            "str.Contains('r')              [✅ char overload — daha hızlı]",
            () => { _ = sentence.Contains('r'); },
            iterations: 5_000_000);

        // ToLower() → yeni string allocate; OrdinalIgnoreCase → sıfır alloc
        const string a = "Performans";
        const string b = "performans";
        DemoRunner.Compare(
            "Büyük/küçük harf duyarsız karşılaştırma",
            "a.ToLower() == b.ToLower()     [❌ 2 geçici string alloc]",
            () => { _ = a.ToLower() == b.ToLower(); },
            "OrdinalIgnoreCase              [✅ sıfır allocate]",
            () => { _ = string.Equals(a, b, StringComparison.OrdinalIgnoreCase); },
            iterations: 5_000_000);

        // string.Create: hedef buffer'ı bir kez allocate eder, içeriği doğrudan yazar
        DemoRunner.Compare(
            "Formatlanmış ID üretme (\"PRD-000042\")",
            "$\"PRD-{id:D6}\"                [❌ interpolation geçici alloc]",
            () =>
            {
                int id = 42;
                _ = $"PRD-{id:D6}";
            },
            "string.Create(...)             [✅ tek allocation, sıfır kopya]",
            () =>
            {
                int id = 42;
                _ = string.Create(10, id, static (span, v) =>
                {
                    "PRD-".CopyTo(span);
                    v.TryFormat(span[4..], out _, "D6");
                });
            },
            iterations: 2_000_000);

        Console.WriteLine("""

  ⚠️  Kurallar:
     • Döngüde += kullanma — StringBuilder ile başlat, ToString() son adımda
     • Char aranıyorsa Contains(char), IndexOf(char) kullan (string overload'dan hızlı)
     • Eşitlik kontrolünde Ordinal / OrdinalIgnoreCase tercih et (kültür bağımsız)
     • string.Create: JSON/CSV satır üretimi gibi hot path'lerde altın kural
""");
    }
}
