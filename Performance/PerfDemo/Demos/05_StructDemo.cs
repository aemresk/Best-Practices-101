namespace PerfDemo.Demos;

// struct vs class: heap baskısını azalt, cache-friendly bellek düzeni
internal static class StructDemo
{
    private const int Count = 50_000;

    // ❌ class: her örnek heap'te ayrı nesne → GC takip eder, pointer indirection
    private sealed class PointClass(double x, double y)
    {
        public double X { get; } = x;
        public double Y { get; } = y;

        public double DistanceTo(PointClass other) =>
            Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    // ✅ readonly struct: değer tipi, inline saklanır, defensive copy engellenir
    private readonly struct PointStruct(double x, double y)
    {
        public readonly double X = x;
        public readonly double Y = y;

        public double DistanceTo(in PointStruct other) =>
            Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    public static void Run()
    {
        DemoRunner.Section("05 — struct vs class: Heap Baskısını Azalt");

        // Dizi oluşturma: class[] → Count ayrı heap nesnesi; struct[] → tek kontiguous blok
        DemoRunner.Compare(
            $"{Count:N0} nokta dizisi oluştur",
            "PointClass[] — her eleman heap [❌ Count adet GC nesnesi]",
            () =>
            {
                var arr = new PointClass[Count];
                for (int i = 0; i < Count; i++) arr[i] = new PointClass(i, i);
                _ = arr[Count - 1].X;
            },
            "PointStruct[] — tek allocation  [✅ cache-friendly, inline]",
            () =>
            {
                var arr = new PointStruct[Count];
                for (int i = 0; i < Count; i++) arr[i] = new PointStruct(i, i);
                _ = arr[Count - 1].X;
            },
            iterations: 500);

        // Hesaplama: class → pointer dereferencing; struct → doğrudan değer erişimi
        var classArr  = Enumerable.Range(0, Count).Select(i => new PointClass(i, i)).ToArray();
        var structArr = Enumerable.Range(0, Count).Select(i => new PointStruct(i, i)).ToArray();
        var origin    = new PointStruct(0, 0);

        DemoRunner.Compare(
            $"Tüm dizi üzerinde mesafe toplamı",
            "class dizisi üzerinde          [❌ pointer chasing, cache miss]",
            () =>
            {
                var o    = new PointClass(0, 0);
                double t = 0;
                foreach (var p in classArr) t += p.DistanceTo(o);
                _ = t;
            },
            "struct dizisi üzerinde         [✅ cache-friendly erişim]",
            () =>
            {
                double t = 0;
                foreach (var p in structArr) t += p.DistanceTo(in origin);
                _ = t;
            },
            iterations: 200);

        Console.WriteLine("""

  ⚠️  Kurallar:
     • Küçük, değer semantikli tipler için struct kullan (≤ ~16 byte idealdir)
     • readonly struct: tüm alanlar readonly → compiler defensive copy engeller
     • in parametresi: büyük struct'ı kopyalamadan geç (ref readonly gibi)
     • Boxing'e dikkat: struct'ı IEnumerable<object>'e atarsan heap'e gider
""");
    }
}
