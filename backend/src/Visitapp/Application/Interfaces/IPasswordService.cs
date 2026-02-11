using Visitapp.Infrastructure.Services;

namespace Visitapp.Application.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        bool IsValidPassword(string password);
        PasswordStrength GetPasswordStrength(string password);
    }
}