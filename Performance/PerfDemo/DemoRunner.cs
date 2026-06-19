using System.Diagnostics;

namespace PerfDemo;

// Her demo için ortak ölçüm altyapısı: süre + allocation + GC sayısı
internal static class DemoRunner
{
    public static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('═', 64));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('═', 64));
    }

    public static void Compare(
        string scenario,
        string labelA,    Action actionA,
        string labelB,    Action actionB,
        int    warmup     = 3,
        int    iterations = 500_000)
    {
        Console.WriteLine($"\n  {scenario}  ({iterations:N0} iterasyon)");
        Measure(labelA, actionA, warmup, iterations);
        Measure(labelB, actionB, warmup, iterations);
    }

    private static void Measure(string label, Action action, int warmup, int iterations)
    {
        for (int i = 0; i < warmup; i++) action();

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();

        var gen0Before  = GC.CollectionCount(0);
        var allocBefore = GC.GetTotalAllocatedBytes(precise: false);
        var sw          = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++) action();

        sw.Stop();

        var allocKb  = (GC.GetTotalAllocatedBytes(precise: false) - allocBefore) / 1024.0;
        var gen0Diff = GC.CollectionCount(0) - gen0Before;

        Console.WriteLine(
            $"    {label,-44} {sw.ElapsedMilliseconds,5}ms | " +
            $"Alloc: {allocKb,8:F1} KB | GC0: {gen0Diff}");
    }
}
