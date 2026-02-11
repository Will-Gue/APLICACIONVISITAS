using Visitapp.Domain.Builders;
using Xunit;

namespace Visitapp.Tests.Domain.Builders
{
    public class UserBuilderTests
    {
        [Fact]
        public void Build_WithAllRequiredFields_CreatesUser()
        {
            // Arrange & Act
            var user = UserBuilder.New()
                .WithFullName("John Doe")
                .WithEmail("john@example.com")
                .WithPhone("1234567890")
                .WithPasswordHash("hashedpassword")
                .Build();

            // Assert
            Assert.NotNull(user);
            Assert.Equal("John Doe", user.FullName);
            Assert.Equal("john@example.com", user.Email);
            Assert.Equal("1234567890", user.Phone);
            Assert.Equal("hashedpassword", user.PasswordHash);
        }

        [Fact]
        public void Build_WithAllFields_CreatesUserWithAllProperties()
        {
            // Arrange & Act
            var user = UserBuilder.New()
                .WithFullName("Jane Smith")
                .WithEmail("jane@example.com")
                .WithPhone("0987654321")
                .WithPasswordHash("hashedpassword123")
                .WithChurch(42)
                .AsVerified()
                .Build();

            // Assert
            Assert.NotNull(user);
            Assert.Equal("Jane Smith", user.FullName);
            Assert.Equal("jane@example.com", user.Email);
            Assert.Equal("0987654321", user.Phone);
            Assert.Equal("hashedpassword123", user.PasswordHash);
            Assert.Equal(42, user.ChurchId);
        }

        [Fact]
        public void Build_WithoutFullName_ThrowsInvalidOperationException()
        {
            // Arrange
            var builder = UserBuilder.New()
                .WithEmail("test@example.com")
                .WithPhone("1234567890")
                .WithPasswordHash("hash");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Contains("Full name is required", exception.Message);
        }

        [Fact]
        public void WithFullName_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = UserBuilder.New();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => builder.WithFullName(null!));
        }

        [Fact]
        public void WithEmail_ConvertsToLowerCase()
        {
            // Arrange & Act
            var user = UserBuilder.New()
                .WithFullName("Test User")
                .WithEmail("TEST@EXAMPLE.COM")
                .WithPhone("1234567890")
                .WithPasswordHash("hash")
                .Build();

            // Assert
            Assert.Equal("test@example.com", user.Email);
        }
    }
}