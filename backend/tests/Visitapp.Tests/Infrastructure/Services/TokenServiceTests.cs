using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Visitapp.Domain.Entities;
using Visitapp.Infrastructure.Services;
using Xunit;

namespace Visitapp.Tests.Infrastructure.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["Jwt:Key"])
                .Returns("test-secret-key-for-jwt-tokens-make-it-long-enough-123456789");
            _mockConfiguration.Setup(x => x["Jwt:ExpirationDays"])
                .Returns("7");

            _tokenService = new TokenService(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateJwtToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");
            var role = "pastor";

            // Act
            var token = _tokenService.GenerateJwtToken(user, role);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT format check
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnsPrincipal()
        {
            // Arrange
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");
            var token = _tokenService.GenerateJwtToken(user, "pastor");

            // Act
            var principal = _tokenService.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal(user.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
            Assert.Equal("pastor", principal.FindFirst(ClaimTypes.Role)?.Value);
        }

        [Fact]
        public void ValidateToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalid.jwt.token";

            // Act
            var principal = _tokenService.ValidateToken(invalidToken);

            // Assert
            Assert.Null(principal);
        }

        [Fact]
        public void GetTokenExpiration_ValidToken_ReturnsExpiration()
        {
            // Arrange
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");
            var token = _tokenService.GenerateJwtToken(user, "pastor");

            // Act
            var expiration = _tokenService.GetTokenExpiration(token);

            // Assert
            Assert.True(expiration > DateTime.UtcNow);
            Assert.True(expiration <= DateTime.UtcNow.AddDays(8)); // Within expected range
        }

        [Fact]
        public void GetUserIdFromToken_ValidToken_ReturnsUserId()
        {
            // Arrange
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");
            var token = _tokenService.GenerateJwtToken(user, "pastor");

            // Act
            var userId = _tokenService.GetUserIdFromToken(token);

            // Assert
            Assert.Equal(user.Id.ToString(), userId);
        }
    }
}