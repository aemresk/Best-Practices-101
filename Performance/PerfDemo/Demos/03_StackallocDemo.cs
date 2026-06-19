namespace PerfDemo.Demos;

// stackalloc: küçük geçici buffer'ları heap yerine stack'te allocate et
internal static class StackallocDemo
{
    public static void Run()
    {
        DemoRunner.Section("03 — stackalloc: Stack Bellek Tahsisi");

        // new byte[] heap'e gider, GC takip eder; stackalloc stack frame'de yaşar
        DemoRunner.Compare(
            "128 byte geçici buffer",
            "new byte[128]                  [❌ heap alloc, GC takip eder]",
            () =>
            {
                var buf = new byte[128];
                buf[0]  = 0xFF;
            },
            "stackalloc byte[128] → Span    [✅ stack — GC yok]",
            () =>
            {
                Span<byte> buf = stackalloc byte[128];
                buf[0] = 0xFF;
            },
            iterations: 2_000_000);

        // number.ToString() heap'te yeni string oluşturur
        // TryFormat ile stackalloc char[] → sıfır alloc
        DemoRunner.Compare(
            "Int → char buffer formatla",
            "number.ToString()              [❌ string heap alloc]",
            () => { _ = 123_456_789.ToString("N0"); },
            "stackalloc + TryFormat         [✅ sıfır allocate]",
            () =>
            {
                Span<char> buf = stackalloc char[32];
                123_456_789.TryFormat(buf, out _, "N0");
            },
            iterations: 2_000_000);

        // Küçük geçici hesaplama tablosu
        DemoRunner.Compare(
            "Geçici int[16] hesaplama tablosu",
            "new int[16]                    [❌ heap alloc]",
            () =>
            {
                var table = new int[16];
                for (int i = 0; i < 16; i++) table[i] = i * i;
                _ = table[15];
            },
            "stackalloc int[16]             [✅ stack]",
            () =>
            {
                Span<int> table = stackalloc int[16];
                for (int i = 0; i < 16; i++) table[i] = i * i;
                _ = table[15];
            },
            iterations: 2_000_000);

        Console.WriteLine("""

  ⚠️  Kurallar:
     • Yalnızca KÜÇÜK, SABİT boyutlar için (≤ ~512 byte önerilen sınır)
     • Büyük stackalloc → StackOverflowException riski
     • Span<T> ile kullan — unsafe blok GEREKMEZ (.NET Core+)
     • Metot dışına taşınamaz — stack frame ömrüyle sınırlı
""");
    }
}
