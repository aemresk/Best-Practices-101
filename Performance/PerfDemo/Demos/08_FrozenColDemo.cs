using System.Collections.Frozen;

namespace PerfDemo.Demos;

// FrozenDictionary / FrozenSet (.NET 8+): salt-okunur, lookup-optimize koleksiyonlar
internal static class FrozenColDemo
{
    // Sabit lookup tablosu — uygulama ömrü boyunca değişmez
    private static readonly Dictionary<string, int>       _dict;
    private static readonly FrozenDictionary<string, int> _frozenDict;
    private static readonly HashSet<string>               _set;
    private static readonly FrozenSet<string>             _frozenSet;
    private static readonly string[]                      _keys;

    static FrozenColDemo()
    {
        // 100 ülke kodu → nüfus (gerçekçi boyut)
        var entries = Enumerable.Range(1, 100)
            .Select(i => ($"ÜLKEKod{i:D3}", i * 1_000_000))
            .ToArray();

        _dict       = entries.ToDictionary(e => e.Item1, e => e.Item2);
        _frozenDict = _dict.ToFrozenDictionary();

        _set        = entries.Select(e => e.Item1).ToHashSet();
        _frozenSet  = _set.ToFrozenSet();

        // Her döngüde 10 farklı key sorgulayacağız
        _keys = entries.Select(e => e.Item1).Take(10).ToArray();
    }

    public static void Run()
    {
        DemoRunner.Section("08 — FrozenDictionary / FrozenSet (.NET 8+)");

        // FrozenDictionary: derleme zamanında mükemmel hash hesaplar → branch-free lookup
        DemoRunner.Compare(
            "Dictionary lookup (10 key × iterasyon)",
            "Dictionary<string,int>         [standart hash lookup]",
            () =>
            {
                foreach (var key in _keys)
                    _dict.TryGetValue(key, out _);
            },
            "FrozenDictionary<string,int>   [✅ mükemmel hash, branch-free]",
            () =>
            {
                foreach (var key in _keys)
                    _frozenDict.TryGetValue(key, out _);
            },
            iterations: 500_000);

        // FrozenSet: üyelik kontrolünde ek optimizasyon
        DemoRunner.Compare(
            "HashSet üyelik kontrolü (10 key × iterasyon)",
            "HashSet<string>.Contains       [standart]",
            () =>
            {
                foreach (var key in _keys)
                    _set.Contains(key);
            },
            "FrozenSet<string>.Contains     [✅ optimize edilmiş]",
            () =>
            {
                foreach (var key in _keys)
                    _frozenSet.Contains(key);
            },
            iterations: 500_000);

        Console.WriteLine("""

  💡 Ne zaman FrozenDictionary / FrozenSet?
     ✅ Uygulama başında bir kez oluşturulan, hiç değişmeyen lookup tabloları
     ✅ Hata kodu → mesaj, ülke kodu → bilgi, konfigürasyon değerleri
     ✅ Lookup başına küçük kazanç ama milyonlarca çağrıda birikir

  ⚠️  Kurallar:
     • ToFrozenDictionary() / ToFrozenSet() → oluşturma pahalıdır — startup'ta bir kez yap
     • Mutasyon mümkün değil — salt okunur veri için tasarlanmıştır
     • StringComparer.OrdinalIgnoreCase ile de çalışır:
       dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase)
""");
    }
}
