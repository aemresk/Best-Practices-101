var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<IUserEmailSender, ConsoleUserEmailSender>();
builder.Services.AddSingleton<UserRegistrationService>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<UserProfileService>();
builder.Services.AddSingleton<PasswordResetService>();

var app = builder.Build();

app.MapGet("/users", (UserStore store) => store.GetAll());

app.MapPost("/register", (RegisterRequest req, UserRegistrationService svc) =>
{
    var user = svc.Register(req);
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPost("/login", (LoginRequest req, AuthenticationService svc) =>
    Results.Ok(new { token = svc.Login(req) }));

app.MapPut("/users/{id:int}/profile", (int id, UpdateProfileRequest req, UserProfileService svc) =>
    svc.UpdateProfile(id, req));

app.MapPost("/password-reset", (ResetPasswordRequest req, PasswordResetService svc) =>
    { svc.RequestReset(req); return Results.Ok(); });

app.MapPost("/password-reset/confirm", (ConfirmResetRequest req, PasswordResetService svc) =>
    { svc.ConfirmReset(req); return Results.Ok(); });

app.Run();
