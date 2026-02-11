using Microsoft.Extensions.Logging;
using Moq;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Interfaces;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Interfaces;
using Visitapp.Domain.Specifications;
using Xunit;

namespace Visitapp.Tests.Application.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _mockLogger = new Mock<ILogger<LoginCommandHandler>>();
            
            _handler = new LoginCommandHandler(
                _mockUnitOfWork.Object,
                _mockTokenService.Object,
                _mockPasswordService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "password123" };
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");
            var token = "jwt-token";
            var expiration = DateTime.UtcNow.AddDays(7);

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync("test@test.com"))
                .ReturnsAsync(user);
            _mockPasswordService.Setup(x => x.VerifyPassword("password123", "hashedpassword"))
                .Returns(true);
            _mockUnitOfWork.Setup(x => x.Users.GetWithRolesAsync(user.Id))
                .ReturnsAsync(user);
            _mockTokenService.Setup(x => x.GenerateJwtToken(user, "user"))
                .Returns(token);
            _mockTokenService.Setup(x => x.GetTokenExpiration(token))
                .Returns(expiration);

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal(user.Email, result.User.Email);
        }

        [Fact]
        public async Task HandleAsync_InvalidCredentials_ThrowsUnauthorizedException()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "wrongpassword" };
            var user = User.Create("Test User", "test@test.com", "1234567890", "hashedpassword");

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync("test@test.com"))
                .ReturnsAsync(user);
            _mockPasswordService.Setup(x => x.VerifyPassword("wrongpassword", "hashedpassword"))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(command));
        }

        [Fact]
        public async Task HandleAsync_UserNotFound_ThrowsUnauthorizedException()
        {
            // Arrange
            var command = new LoginCommand { Email = "nonexistent@test.com", Password = "password123" };

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync("nonexistent@test.com"))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(command));
        }
    }
}