using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Visitapp.Controllers;
using Visitapp.Data;
using Visitapp.Models;
using Xunit;

namespace Visitapp.Tests.Controllers
{
    public class AuthControllerTests : IDisposable
    {
        private readonly VisitAppContext _context;
        private readonly AuthController _controller;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AuthControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<VisitAppContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VisitAppContext(options);
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mock
            _mockConfiguration.Setup(x => x["Jwt:Key"])
                .Returns("your-secret-key-here-make-it-long-and-secure-123456789");

            _controller = new AuthController(_context, _mockLogger.Object, _mockConfiguration.Object);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Add test roles
            var userRole = new Roles { RoleName = "user" };
            _context.Roles.Add(userRole);

            // Add test user
            var testUser = new Users
            {
                FullName = "Test User",
                Email = "test@test.com",
                Phone = "1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(testUser);
            _context.SaveChanges();

            // Add user role relationship
            var userRoleRelation = new UserRoles
            {
                UserId = testUser.UserId,
                RoleId = userRole.RoleId,
                AssignedDate = DateTime.UtcNow
            };
            _context.UserRoles.Add(userRoleRelation);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "",
                Password = "password123"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FullName = "New User",
                Email = "newuser@test.com",
                Phone = "9876543210",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Verify user was created
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@test.com");
            Assert.NotNull(user);
            Assert.Equal("New User", user.FullName);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FullName = "Duplicate User",
                Email = "test@test.com", // This email already exists
                Phone = "5555555555",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FullName = "Test User",
                Email = "invalid-email",
                Phone = "1111111111",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShortPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FullName = "Test User",
                Email = "short@test.com",
                Phone = "2222222222",
                Password = "123" // Too short
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}