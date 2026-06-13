// ✅ Her Bird yedeği güvenle kullanılabilir — hiçbir exception fırlamaz
public class BirdTrainer
{
    private readonly List<Bird> _birds = new()
    {
        new Sparrow { Name = "Serçe" },
        new Eagle   { Name = "Kartal" },
        new Penguin { Name = "Penguen" }  // ✅ Artık güvenli
    };

    public List<object> TrainAll()
    {
        var results = new List<object>();
        foreach (var bird in _birds)
        {
            results.Add(new { bird.Name, Eat = bird.Eat(), Describe = bird.Describe() });

            // ✅ Uçabiliyorsa uçur — tip kontrolü değil, capability kontrolü
            if (bird is IFlyable flyable)
                results.Add(new { bird.Name, Action = flyable.Fly() });

            if (bird is ISwimmable swimmable)
                results.Add(new { bird.Name, Action = swimmable.Swim() });
        }
        return results;
    }

    public List<string> GetBirds() => _birds.Select(b => b.Name).ToList();
}
