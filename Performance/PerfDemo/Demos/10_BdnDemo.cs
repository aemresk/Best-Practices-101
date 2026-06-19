using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Running;

namespace PerfDemo.Demos;

// BenchmarkDotNet: doğru performans ölçümü için sektör standardı araç
internal static class BdnDemo
{
    public static void Run()
    {
        DemoRunner.Section("10 — BenchmarkDotNet: Doğru Performans Ölçümü");

        Console.WriteLine("""

  Bu demoda kullandığımız Stopwatch + GC ölçümü geliştirme sırasında hızlı
  geribildirim için yeterlidir. Gerçek karar vermek için BenchmarkDotNet gerekir:

  ✅ JIT ısınması (warmup) otomatik
  ✅ Outlier tespiti ve istatistik (ortalama, medyan, stddev, p99)
  ✅ [MemoryDiagnoser] → allocation (B) ve GC koleksiyonları
  ✅ [DisassemblyDiagnoser] → JIT tarafından üretilen assembly
  ✅ Otomatik Release modu kontrolü (Debug'da uyarı verir)

  Örnek benchmark sınıfı:
""");

        Console.WriteLine("""
  ┌────────────────────────────────────────────────────────────┐
  │  [MemoryDiagnoser]                                         │
  │  [RankColumn]                                              │
  │  public class StringBenchmarks                             │
  │  {                                                         │
  │      private const string Text = "Performans demo...";    │
  │                                                            │
  │      [Benchmark(Baseline = true)]                          │
  │      public string  WithSubstring()    => Text.Substring(12, 5);
  │                                                            │
  │      [Benchmark]                                           │
  │      public ReadOnlySpan<char> WithSpan() =>               │
  │          Text.AsSpan(12, 5);                               │
  │                                                            │
  │      [Benchmark]                                           │
  │      public bool ContainsString() => Text.Contains("Span");│
  │                                                            │
  │      [Benchmark]                                           │
  │      public bool ContainsChar()   => Text.Contains('S');   │
  │  }                                                         │
  └────────────────────────────────────────────────────────────┘
""");

        Console.WriteLine("""
  BDN çıktısı örneği:

  | Method          |      Mean | Ratio | Allocated |
  |-----------------|----------:|------:|----------:|
  | WithSubstring   | 12.34 ns  |  1.00 |      40 B |
  | WithSpan        |  1.23 ns  |  0.10 |       0 B |
  | ContainsString  |  8.10 ns  |  0.66 |       0 B |
  | ContainsChar    |  3.05 ns  |  0.25 |       0 B |

  Çalıştırmak için:
    dotnet run -c Release -- --benchmark

  Belirli bir class:
    dotnet run -c Release -- --filter *StringBenchmarks*
""");
    }

    // BenchmarkDotNet tarafından çalıştırılacak gerçek benchmark sınıfı
    [MemoryDiagnoser]
    [RankColumn]
    public class StringBenchmarks
    {
        private const string Text = "Performans için doğru araçları kullanmak önemlidir — Span<T>.";

        [Benchmark(Baseline = true)]
        public string WithSubstring() => Text.Substring(12, 5);

        [Benchmark]
        public ReadOnlySpan<char> WithSpan() => Text.AsSpan(12, 5);

        [Benchmark]
        public bool ContainsString() => Text.Contains("Span");

        [Benchmark]
        public bool ContainsChar() => Text.Contains('S');

        [Benchmark]
        public bool CompareOrdinal() => string.Equals(Text, Text, StringComparison.Ordinal);

        [Benchmark]
        public bool CompareCurrentCulture() => string.Equals(Text, Text, StringComparison.CurrentCulture);
    }
}
