public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UpdateProfileRequest(string? Bio, string? AvatarUrl);
public record ResetPasswordRequest(string Email);
public record ConfirmResetRequest(string Token, string NewPassword);
