using System.Buffers;

namespace PerfDemo.Demos;

// ArrayPool<T>: diziyi her seferinde yeni allocate etmek yerine kirala/iade et
internal static class ArrayPoolDemo
{
    public static void Run()
    {
        DemoRunner.Section("02 — ArrayPool<T>: GC Baskısını Azalt");

        // 4 KB buffer: ağ/dosya işlemlerinde sık kullanılan boyut
        DemoRunner.Compare(
            "4 KB geçici buffer",
            "new byte[4096]                 [❌ her seferinde heap alloc]",
            () =>
            {
                var buf = new byte[4096];
                buf[0]  = 1; // simüle kullanım
            },
            "ArrayPool.Rent / Return        [✅ havuzdan kirala, iade et]",
            () =>
            {
                var buf = ArrayPool<byte>.Shared.Rent(4096);
                try   { buf[0] = 1; }
                finally { ArrayPool<byte>.Shared.Return(buf); }
            },
            iterations: 200_000);

        // 85 KB üstü → Large Object Heap (LOH) → pahalı GC2 koleksiyonu
        DemoRunner.Compare(
            "100 KB büyük buffer (LOH eşiği üstü)",
            "new byte[102400]               [❌ LOH'a gider, GC2 koleksiyon]",
            () =>
            {
                var buf = new byte[102_400];
                buf[0]  = 1;
            },
            "ArrayPool.Rent(102400)         [✅ LOH baskısı yok]",
            () =>
            {
                var buf = ArrayPool<byte>.Shared.Rent(102_400);
                try   { buf[0] = 1; }
                finally { ArrayPool<byte>.Shared.Return(buf); }
            },
            iterations: 50_000);

        Console.WriteLine("""

  ⚠️  Kurallar:
     • Return'ü MUTLAKA finally bloğunda çağır — yoksa havuz tükenir
     • Rent(N) → N'den BÜYÜK bir dizi döner; Length değil Rent parametresini kullan
     • Hassas veri (şifre, token) içeriyorsa: Return(buf, clearArray: true)
     • 85 KB üstü diziler Large Object Heap'e gider → ArrayPool ile kaçın
""");
    }
}
