// ✅ LSP: Temel sınıf yalnızca TÜM kuşların yapabileceği davranışları içeriyor
public abstract class Bird
{
    public required string Name { get; set; }
    public abstract string Eat();
    public abstract string Describe();
}

// ✅ Uçma yeteneği ayrı bir interface — uçamayan kuşlar implement etmez
public interface IFlyable
{
    string Fly();
}

// ✅ Yüzme yeteneği ayrı interface
public interface ISwimmable
{
    string Swim();
}

// ✅ Bird'in tüm sözleşmelerini (Eat + Describe) tam karşılıyor + IFlyable ekliyor
public class Sparrow : Bird, IFlyable
{
    public override string Eat()      => $"{Name} tohum yiyor";
    public override string Describe() => $"{Name} - Küçük, hızlı bir kuş";
    public string Fly()               => $"{Name} hızla kanat çırparak uçuyor";
}

// ✅ Bird + IFlyable + ISwimmable — LSP uyumlu
public class Eagle : Bird, IFlyable
{
    public override string Eat()      => $"{Name} avını kaptı";
    public override string Describe() => $"{Name} - Güçlü bir yırtıcı";
    public string Fly()               => $"{Name} termal hava akımında süzülüyor";
}

// ✅ Penguen Bird'den türüyor, IFlyable implement ETMİYOR — LSP korunuyor
public class Penguin : Bird, ISwimmable
{
    public override string Eat()      => $"{Name} balık yiyor";
    public override string Describe() => $"{Name} - Antarktika'nın hakimi";
    public string Swim()              => $"{Name} suda 36 km/s hızla yüzüyor";
}
