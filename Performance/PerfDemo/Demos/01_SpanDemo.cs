namespace PerfDemo.Demos;

// Span<T> ve ReadOnlySpan<char>: heap'e dokunmadan dilim işlemleri
internal static class SpanDemo
{
    private const string Csv = "İstanbul,Ankara,İzmir,Bursa,Antalya,Adana,Trabzon,Konya";

    public static void Run()
    {
        DemoRunner.Section("01 — Span<T>: Sıfır Kopya Dilim İşlemleri");

        // string.Substring her seferinde yeni bir string nesnesi allocate eder
        DemoRunner.Compare(
            "String dilimi",
            "str.Substring(9, 6)           [❌ yeni string allocate]",
            () => { _ = Csv.Substring(9, 6); },
            "str.AsSpan().Slice(9, 6)       [✅ sıfır allocate]",
            () => { _ = Csv.AsSpan().Slice(9, 6); },
            iterations: 2_000_000);

        // string.Split: hem dizi hem de her bir eleman için heap allocate
        DemoRunner.Compare(
            "İlk token'ı bul (CSV parse)",
            "str.Split(',')[0]              [❌ string[] + string alloc]",
            () => { _ = Csv.Split(',')[0]; },
            "AsSpan → IndexOf → Slice       [✅ sıfır allocate]",
            () =>
            {
                var span  = Csv.AsSpan();
                var comma = span.IndexOf(',');
                _         = comma >= 0 ? span[..comma] : span;
            },
            iterations: 2_000_000);

        // string.Trim() → yeni string; AsSpan().Trim() → sıfır alloc
        const string padded = "   42   ";
        DemoRunner.Compare(
            "Boşluk temizle + sayı parse",
            "str.Trim() → int.Parse(str)    [❌ Trim yeni string döner]",
            () => { _ = int.Parse(padded.Trim()); },
            "AsSpan().Trim() → int.Parse    [✅ sıfır allocate]",
            () => { _ = int.Parse(padded.AsSpan().Trim()); },
            iterations: 2_000_000);

        Console.WriteLine("""

  ⚠️  Kurallar:
     • ReadOnlySpan<char> asla heap'e kaçmaz — ref struct olduğu için
     • Span sonucu saklayamazsın (field, async, lambda) — yalnızca sync stack'te
     • MemoryExtensions: AsSpan, AsMemory, TrimStart, IndexOf, Split (net8+)
""");
    }
}
