var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BirdTrainer>();
var app = builder.Build();

app.MapGet("/birds", (BirdTrainer t) => t.GetBirds());
app.MapGet("/train", (BirdTrainer t) => t.TrainAll());

app.Run();
