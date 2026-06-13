var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UserManager>();
var app = builder.Build();

app.MapGet("/users", (UserManager um) => um.GetAll());
app.MapPost("/register", (RegisterRequest req, UserManager um) => um.Register(req));
app.MapPost("/login", (LoginRequest req, UserManager um) => um.Login(req));
app.MapPut("/users/{id:int}/profile", (int id, UpdateProfileRequest req, UserManager um) => um.UpdateProfile(id, req));
app.MapPost("/password-reset", (ResetPasswordRequest req, UserManager um) => { um.RequestPasswordReset(req); return Results.Ok(); });
app.MapPost("/password-reset/confirm", (ConfirmResetRequest req, UserManager um) => { um.ConfirmPasswordReset(req); return Results.Ok(); });

app.Run();
