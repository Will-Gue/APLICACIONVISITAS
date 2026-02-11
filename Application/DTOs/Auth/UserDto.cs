namespace Visitapp.Application.DTOs.Auth;

public class UserDto
{
    public int UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = "user";
}
