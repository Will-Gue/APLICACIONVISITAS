namespace Visitapp.Application.DTOs.Auth;

public class RegisterDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Password { get; set; }
}
