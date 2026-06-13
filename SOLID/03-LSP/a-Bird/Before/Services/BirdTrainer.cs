// ❌ LSP İHLALİ: Bird listesinde Penguin olduğunda kod çöküyor
// Çalışan kod, Bird'i Penguin ile değiştirince bozuluyor — bu tam LSP ihlali
public class BirdTrainer
{
    private readonly List<Bird> _birds = new()
    {
        new Sparrow { Name = "Serçe" },
        new Eagle   { Name = "Kartal" },
        new Penguin { Name = "Penguen" }  // ❌ Bu satır BirdTrainer'ı patlatır
    };

    public List<string> TrainAll()
    {
        var results = new List<string>();
        foreach (var bird in _birds)
        {
            results.Add(bird.Eat());
            // ❌ Penguin gelince burada NotSupportedException
            results.Add(bird.Fly());
        }
        return results;
    }

    public List<string> GetBirds() => _birds.Select(b => b.Name).ToList();
}
