// ❌ LSP İHLALİ: Bird sözleşmesi "tüm kuşlar uçabilir" varsayımını içeriyor
public abstract class Bird
{
    public required string Name { get; set; }
    public abstract string Fly();    // ❌ Penguenler uçamaz — bu sözleşme her alt sınıf için geçersiz
    public abstract string Eat();
}

// ✅ Uçabilen kuş — sorun yok
public class Sparrow : Bird
{
    public override string Fly() => $"{Name} hızla kanat çırparak uçuyor";
    public override string Eat() => $"{Name} tohum yiyor";
}

// ✅ Uçabilen kuş — sorun yok
public class Eagle : Bird
{
    public override string Fly() => $"{Name} termal hava akımında süzülüyor";
    public override string Eat() => $"{Name} avını kaptı";
}

// ❌ LSP İHLALİ: Penguen Bird'den türüyor ama uçamıyor
// Bird yerine Penguin kullanılabilir mi? HAYIR — Fly() çağrısı exception fırlatır
public class Penguin : Bird
{
    public override string Fly()
    {
        // ❌ Üst sınıf sözleşmesini bozuyor — çağıran kod bunu beklemez
        throw new NotSupportedException($"{Name} uçamaz!");
    }
    public override string Eat() => $"{Name} balık yiyor";
    public string Swim() => $"{Name} suda yüzüyor";
}
