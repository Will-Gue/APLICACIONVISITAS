namespace Visitapp.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserResponse User { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? ChurchId { get; set; }
    }
}