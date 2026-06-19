namespace PerfDemo.Demos;

// Lazy<T> ve LazyInitializer: geciktirilmiş ve thread-safe başlatma
internal static class LazyInitDemo
{
    // Başlatılması pahalı kaynak (örn: DB schema cache, büyük konfigürasyon)
    private sealed class ExpensiveResource
    {
        public readonly int[] Data;
        public ExpensiveResource()
        {
            Data = new int[50_000];
            Array.Fill(Data, 42);
        }
    }

    public static void Run()
    {
        DemoRunner.Section("09 — Lazy<T>: Geciktirilmiş Başlatma");

        // Eager vs Lazy ilk başlatma maliyeti
        Console.WriteLine("\n  İlk başlatma maliyeti karşılaştırması:");
        Console.WriteLine();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var eager = new ExpensiveResource();            // uygulama başlangıcında ödenir
        sw.Stop();
        _ = eager;
        Console.WriteLine($"    Eager (her zaman ödenir)           {sw.ElapsedMilliseconds,4}ms");

        var lazy = new Lazy<ExpensiveResource>(() => new ExpensiveResource());
        sw.Restart();
        _ = lazy.Value;                                 // yalnızca ilk erişimde
        sw.Stop();
        Console.WriteLine($"    Lazy — ilk erişim (Value)          {sw.ElapsedMilliseconds,4}ms");

        sw.Restart();
        _ = lazy.Value;                                 // sonraki erişimler: neredeyse ücretsiz
        sw.Stop();
        Console.WriteLine($"    Lazy — sonraki erişim (cache)         {sw.ElapsedTicks} tick");

        // Sonraki erişimler için: Lazy<T>.Value vs LazyInitializer (overhead karşılaştırması)
        ExpensiveResource? initField = null;
        var lazyForBench = new Lazy<ExpensiveResource>(() => new ExpensiveResource());
        _ = lazyForBench.Value; // ısın

        DemoRunner.Compare(
            "Sonraki erişimlerin overhead'i",
            "Lazy<T>.Value                  [volatile read]",
            () => { _ = lazyForBench.Value; },
            "LazyInitializer.EnsureInitialized [null check + volatile]",
            () => { LazyInitializer.EnsureInitialized(ref initField, () => new ExpensiveResource()); },
            iterations: 5_000_000);

        Console.WriteLine("""

  💡 Hangi pattern ne zaman?
     ✅ Lazy<T>                        → tek nesne, basit kullanım (singleton, servis)
     ✅ LazyInitializer.EnsureInitialized → field bazlı, birden fazla örnek, daha düşük overhead
     ✅ static readonly Lazy<T>         → global singleton, thread-safe, uygulama ömrü boyunca

  ⚠️  Kurallar:
     • Lazy<T> default: LazyThreadSafetyMode.ExecutionAndPublication (thread-safe)
     • LazyInitializer: factory birden fazla thread'de çalışabilir (racing), yalnızca biri kazanır
     • Lazy içinde exception → her Value erişiminde yeniden fırlatılır (ExecutionAndPublication)
""");
    }
}
