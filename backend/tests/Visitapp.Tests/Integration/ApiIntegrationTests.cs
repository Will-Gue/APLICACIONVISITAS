using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Visitapp.Data;
using Visitapp.Models;
using Xunit;

namespace Visitapp.Tests.Integration
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database context
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<VisitAppContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database for testing
                    services.AddDbContext<VisitAppContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });

                    // Build service provider and seed data
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<VisitAppContext>();
                    SeedTestData(context);
                });
            });

            _client = _factory.CreateClient();
        }

        private static void SeedTestData(VisitAppContext context)
        {
            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Add test roles
            var userRole = new Roles { RoleName = "user" };
            context.Roles.Add(userRole);

            // Add test user
            var testUser = new Users
            {
                FullName = "Integration Test User",
                Email = "integration@test.com",
                Phone = "1111111111",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(testUser);
            context.SaveChanges();

            // Add user role relationship
            var userRoleRelation = new UserRoles
            {
                UserId = testUser.UserId,
                RoleId = userRole.RoleId,
                AssignedDate = DateTime.UtcNow
            };
            context.UserRoles.Add(userRoleRelation);
            context.SaveChanges();
        }

        [Fact]
        public async Task Get_Swagger_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/swagger");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Post_Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginData = new
            {
                Email = "integration@test.com",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseContent);
            Assert.Contains("user", responseContent);
        }

        [Fact]
        public async Task Post_Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginData = new
            {
                Email = "integration@test.com",
                Password = "wrongpassword"
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_Register_ValidData_ReturnsSuccess()
        {
            // Arrange
            var registerData = new
            {
                FullName = "New Integration User",
                Email = "newintegration@test.com",
                Phone = "2222222222",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseContent);
            Assert.Contains("user", responseContent);
        }

        [Fact]
        public async Task Post_Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var registerData = new
            {
                FullName = "Duplicate User",
                Email = "integration@test.com", // This email already exists
                Phone = "3333333333",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Get_Users_WithoutAuth_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            // With in-memory database, this should work
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}