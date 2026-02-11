using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.Common;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Controllers.V2;
using Xunit;

namespace Visitapp.Tests.Controllers.V2
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ValidCommand_ReturnsOkResult()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "password123" };
            var expectedResponse = new AuthResponse
            {
                Token = "jwt-token",
                Message = "Login successful",
                User = new UserResponse { Email = "test@test.com", FullName = "Test User" }
            };

            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("jwt-token", response.Token);
            Assert.Equal("Login successful", response.Message);
        }

        [Fact]
        public async Task Login_UnauthorizedAccess_ReturnsUnauthorized()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "wrongpassword" };
            
            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(command);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(unauthorizedResult.Value);
            Assert.Equal("Authentication Failed", problemDetails.Title);
            Assert.Contains("Invalid credentials", problemDetails.Detail);
        }

        [Fact]
        public async Task Login_GeneralException_ReturnsInternalServerError()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "password123" };
            
            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.Login(command);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(serverErrorResult.Value);
            Assert.Equal("Internal Server Error", problemDetails.Title);
        }

        [Fact]
        public async Task Register_ValidCommand_ReturnsCreatedResult()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "New User",
                Email = "new@test.com", 
                Phone = "1234567890",
                Password = "password123" 
            };
            var expectedResponse = new AuthResponse
            {
                Token = "jwt-token",
                Message = "Registration successful",
                User = new UserResponse { Email = "new@test.com", FullName = "New User" }
            };

            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(command);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<AuthResponse>(createdResult.Value);
            Assert.Equal("jwt-token", response.Token);
            Assert.Equal("Registration successful", response.Message);
        }

        [Fact]
        public async Task Register_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "",
                Email = "invalid-email", 
                Phone = "123",
                Password = "weak" 
            };
            
            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Invalid email format"));

            // Act
            var result = await _controller.Register(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
            Assert.Equal("Invalid Data", problemDetails.Title);
            Assert.Contains("Invalid email format", problemDetails.Detail);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                FullName = "Test User",
                Email = "existing@test.com", 
                Phone = "1234567890",
                Password = "password123" 
            };
            
            _mockMediator.Setup(m => m.SendAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Email already registered"));

            // Act
            var result = await _controller.Register(command);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(conflictResult.Value);
            Assert.Equal("Conflict", problemDetails.Title);
            Assert.Contains("Email already registered", problemDetails.Detail);
        }

        [Fact]
        public async Task GetCurrentUser_ValidToken_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "123"),
                new(ClaimTypes.Email, "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = principal
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>(); // No NameIdentifier claim
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = principal
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(unauthorizedResult.Value);
            Assert.Equal("Invalid Token", problemDetails.Title);
        }

        [Fact]
        public async Task GetCurrentUser_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "invalid-id") // Non-numeric ID
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = principal
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(unauthorizedResult.Value);
            Assert.Equal("Invalid Token", problemDetails.Title);
        }
    }
}