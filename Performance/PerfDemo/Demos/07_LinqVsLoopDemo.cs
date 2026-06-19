namespace PerfDemo.Demos;

// LINQ vs for döngüsü: hot path'te ne zaman hangisi?
internal static class LinqVsLoopDemo
{
    private static readonly int[] _numbers =
        Enumerable.Range(1, 10_000).ToArray();

    private static readonly List<int> _list =
        Enumerable.Range(1, 10_000).ToList();

    public static void Run()
    {
        DemoRunner.Section("07 — LINQ vs for Döngüsü");

        // LINQ.Sum → IEnumerable<int> delegate overhead; for → doğrudan dizi erişimi
        DemoRunner.Compare(
            "Dizi toplamı (10.000 eleman)",
            "array.Sum()                    [❌ delegate, enumerator overhead]",
            () => { _ = _numbers.Sum(); },
            "for döngüsü                    [✅ doğrudan erişim]",
            () =>
            {
                long total = 0;
                var  nums  = _numbers;
                for (int i = 0; i < nums.Length; i++) total += nums[i];
                _ = total;
            },
            iterations: 50_000);

        // Where + ToList → lambda alloc + List alloc; manuel → yalnızca List alloc
        DemoRunner.Compare(
            "Filtreleme + materialize (10.000 eleman)",
            ".Where(x > 5000).ToList()      [❌ lambda + enumerator alloc]",
            () => { _ = _list.Where(x => x > 5000).ToList(); },
            "Manuel foreach → List.Add      [✅ tek List alloc]",
            () =>
            {
                var result = new List<int>(_list.Count / 2);
                foreach (var x in _list)
                    if (x > 5000) result.Add(x);
                _ = result;
            },
            iterations: 20_000);

        // LINQ.Any: büyük koleksiyonda delegate overhead; manuel for break daha hızlı
        DemoRunner.Compare(
            "İlk koşulu sağlayan eleman (erken çıkış)",
            ".FirstOrDefault(x > 9990)      [❌ delegate overhead]",
            () => { _ = _numbers.FirstOrDefault(x => x > 9990); },
            "for + break                    [✅ erken çıkış açık]",
            () =>
            {
                var nums = _numbers;
                int found = 0;
                for (int i = 0; i < nums.Length; i++)
                    if (nums[i] > 9990) { found = nums[i]; break; }
                _ = found;
            },
            iterations: 200_000);

        Console.WriteLine("""

  💡 Ne zaman LINQ, ne zaman for?
     ✅ LINQ: Okunabilirlik öncelikli, tek seferlik sorgu, karmaşık join/group işlemleri
     ✅ for:  Hot path, sıkı döngü, allocation'ın kritik olduğu yüksek-throughput kod
     ❌ Kaçın: LINQ'u hot path'te her çağrıda .ToList() / .ToArray() ile sonlandırma
     ❌ Kaçın: Select + new anonim tip → her eleman için heap allocation
""");
    }
}
