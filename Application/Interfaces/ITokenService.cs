using System.Security.Claims;
using Visitapp.Domain.Entities;

namespace Visitapp.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, string role);
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiration(string token);
        string? GetUserIdFromToken(string token);
    }
}