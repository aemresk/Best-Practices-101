using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace PerfDemo.Demos;

// ObjectPool<T>: pahalı oluşturulan nesneleri tekrar kullan
internal static class ObjectPoolDemo
{
    // Singleton pool — oluşturma maliyeti bir kez ödenir
    private static readonly ObjectPool<StringBuilder> _sbPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool(
            initialCapacity:         256,
            maximumRetainedCapacity: 4096);

    public static void Run()
    {
        DemoRunner.Section("06 — ObjectPool<T>: Pahalı Nesneleri Yeniden Kullan");

        DemoRunner.Compare(
            "StringBuilder ile 20 parça birleştir + ToString",
            "new StringBuilder() her seferinde [❌ alloc + GC baskısı]",
            () =>
            {
                var sb = new StringBuilder(256);
                for (int i = 0; i < 20; i++) sb.Append("parça-").Append(i);
                _ = sb.ToString();
            },
            "ObjectPool<StringBuilder>         [✅ kirala → kullan → iade]",
            () =>
            {
                var sb = _sbPool.Get();
                try
                {
                    for (int i = 0; i < 20; i++) sb.Append("parça-").Append(i);
                    _ = sb.ToString();
                }
                finally
                {
                    _sbPool.Return(sb); // pool Clear() çağırır, sonraki kullanıma hazır
                }
            },
            iterations: 200_000);

        Console.WriteLine("""

  💡 Ne zaman ObjectPool kullanılır?
     ✅ StringBuilder, MemoryStream, HttpClient, DB connection gibi pahalı nesneler
     ✅ Yüksek throughput senaryolarında (web sunucusu, mesaj işleyici)
     ❌ Ucuz, kısa ömürlü nesneler için gereksiz karmaşıklık

  ⚠️  Kurallar:
     • Get() → kullan → Return() → finally bloğu ZORUNLU
     • Return'den ÖNCE içeriği temizle (pool policy otomatik yapar: StringBuilderPool)
     • Pool'u Singleton olarak tut — her seferinde new DefaultObjectPoolProvider() oluşturma
     • Thread-safe: DefaultObjectPool<T> çoklu thread için güvenli
""");
    }
}
