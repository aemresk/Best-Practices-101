using BenchmarkDotNet.Running;
using PerfDemo.Demos;

// Demo modu:   dotnet run -c Release
// BDN modu:    dotnet run -c Release -- --benchmark

if (args.Contains("--benchmark"))
{
    BenchmarkRunner.Run<BdnDemo.StringBenchmarks>();
    return;
}

Console.WriteLine("""
╔══════════════════════════════════════════════════════════════════╗
║         .NET Performance Best Practices — Demo                  ║
║                                                                 ║
║  Her ölçüm:  süre (ms) | toplam allocation (KB) | GC Gen0 sayısı║
║  Doğru ölçüm: dotnet run -c Release                             ║
╚══════════════════════════════════════════════════════════════════╝
""");

SpanDemo.Run();
ArrayPoolDemo.Run();
StackallocDemo.Run();
StringOpsDemo.Run();
StructDemo.Run();
ObjectPoolDemo.Run();
LinqVsLoopDemo.Run();
FrozenColDemo.Run();
LazyInitDemo.Run();
BdnDemo.Run();

Console.WriteLine("""

══════════════════════════════════════════════════════════════════
  Tüm demolar tamamlandı.
  BenchmarkDotNet için: dotnet run -c Release -- --benchmark
══════════════════════════════════════════════════════════════════
""");
