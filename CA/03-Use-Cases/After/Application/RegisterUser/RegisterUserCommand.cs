namespace Application.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string FullName);
public record RegisterUserResult(int UserId, string Email);
