using Visitapp.Application.DTOs.Auth;

namespace Visitapp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> GetCurrentUserAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}