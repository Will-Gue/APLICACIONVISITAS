using Visitapp.Infrastructure.Services;
using Xunit;

namespace Visitapp.Tests.Infrastructure.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ValidPassword_ReturnsHash()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var hash = _passwordService.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotEqual(password, hash);
            Assert.True(hash.StartsWith("$2"));
        }

        [Fact]
        public void HashPassword_NullPassword_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _passwordService.HashPassword(null!));
        }

        [Fact]
        public void HashPassword_EmptyPassword_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _passwordService.HashPassword(""));
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "testpassword123";
            var hash = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "testpassword123";
            var wrongPassword = "wrongpassword";
            var hash = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_NullPassword_ReturnsFalse()
        {
            // Arrange
            var hash = _passwordService.HashPassword("testpassword");

            // Act
            var result = _passwordService.VerifyPassword(null!, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_NullHash_ReturnsFalse()
        {
            // Act
            var result = _passwordService.VerifyPassword("password", null!);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("Password123!", true)] // Strong: upper, lower, number, special
        [InlineData("password123", true)]  // Medium: lower, number (meets 3/4 criteria)
        [InlineData("PASSWORD123", true)]  // Medium: upper, number (meets 3/4 criteria)
        [InlineData("Password123", true)]  // Strong: upper, lower, number (meets 3/4 criteria)
        [InlineData("password", false)]    // Weak: only lower
        [InlineData("123456", false)]      // Weak: only numbers
        [InlineData("", false)]            // Invalid: empty
        [InlineData("12", false)]          // Invalid: too short
        public void IsValidPassword_VariousPasswords_ReturnsExpectedResult(string password, bool expected)
        {
            // Act
            var result = _passwordService.IsValidPassword(password);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("", PasswordStrength.VeryWeak)]
        [InlineData("123", PasswordStrength.VeryWeak)]
        [InlineData("password", PasswordStrength.Weak)]
        [InlineData("Password", PasswordStrength.Weak)]
        [InlineData("password123", PasswordStrength.Medium)]
        [InlineData("Password123", PasswordStrength.Strong)]
        [InlineData("Password123!", PasswordStrength.VeryStrong)]
        [InlineData("VeryLongPassword123!@#", PasswordStrength.VeryStrong)]
        public void GetPasswordStrength_VariousPasswords_ReturnsCorrectStrength(string password, PasswordStrength expected)
        {
            // Act
            var result = _passwordService.GetPasswordStrength(password);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsValidPassword_TooLongPassword_ReturnsFalse()
        {
            // Arrange
            var longPassword = new string('a', 101); // 101 characters

            // Act
            var result = _passwordService.IsValidPassword(longPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_SamePasswordTwice_ReturnsDifferentHashes()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var hash1 = _passwordService.HashPassword(password);
            var hash2 = _passwordService.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // BCrypt uses salt, so hashes should be different
            Assert.True(_passwordService.VerifyPassword(password, hash1));
            Assert.True(_passwordService.VerifyPassword(password, hash2));
        }
    }
}