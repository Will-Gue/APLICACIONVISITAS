using Microsoft.Extensions.Logging;
using Moq;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Interfaces;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Interfaces;
using Xunit;

namespace Visitapp.Tests.Application.Commands
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<ILogger<RegisterCommandHandler>> _mockLogger;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _mockLogger = new Mock<ILogger<RegisterCommandHandler>>();
            
            _handler = new RegisterCommandHandler(
                _mockUnitOfWork.Object,
                _mockTokenService.Object,
                _mockPasswordService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidData_ReturnsAuthResponse()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "Test User",
                Email = "test@test.com", 
                Phone = "1234567890",
                Password = "password123" 
            };
            var hashedPassword = "hashedpassword";
            var user = User.Create("Test User", "test@test.com", "1234567890", hashedPassword);
            var token = "jwt-token";
            var expiration = DateTime.UtcNow.AddDays(7);

            _mockPasswordService.Setup(x => x.IsValidPassword("password123"))
                .Returns(true);
            _mockPasswordService.Setup(x => x.HashPassword("password123"))
                .Returns(hashedPassword);
            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync("test@test.com"))
                .ReturnsAsync((User?)null);
            _mockUnitOfWork.Setup(x => x.Users.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockTokenService.Setup(x => x.GenerateJwtToken(It.IsAny<User>(), "user"))
                .Returns(token);
            _mockTokenService.Setup(x => x.GetTokenExpiration(token))
                .Returns(expiration);

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal("Registration successful", result.Message);
            Assert.Equal("test@test.com", result.User.Email);
        }

        [Fact]
        public async Task HandleAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "Test User",
                Email = "existing@test.com", 
                Phone = "1234567890",
                Password = "password123" 
            };
            var existingUser = User.Create("Existing User", "existing@test.com", "0987654321", "hash");

            _mockPasswordService.Setup(x => x.IsValidPassword("password123"))
                .Returns(true);
            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync("existing@test.com"))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            Assert.Contains("already registered", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_InvalidPassword_ThrowsArgumentException()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "Test User",
                Email = "test@test.com", 
                Phone = "1234567890",
                Password = "123" // Too weak
            };

            _mockPasswordService.Setup(x => x.IsValidPassword("123"))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
            Assert.Contains("Password does not meet", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_EmptyFullName_ThrowsArgumentException()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "",
                Email = "test@test.com", 
                Phone = "1234567890",
                Password = "password123" 
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
            Assert.Contains("Full name is required", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "Test User",
                Email = "invalid-email", 
                Phone = "1234567890",
                Password = "password123" 
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
            Assert.Contains("Invalid email format", exception.Message);
        }
    }
}