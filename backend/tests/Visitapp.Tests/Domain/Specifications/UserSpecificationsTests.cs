using Visitapp.Domain.Entities;
using Visitapp.Domain.Specifications;
using Xunit;

namespace Visitapp.Tests.Domain.Specifications
{
    public class UserSpecificationsTests
    {
        [Fact]
        public void UserByEmailSpecification_WithValidEmail_CreatesCorrectExpression()
        {
            // Arrange
            var email = "Test@Example.Com";
            var spec = new UserByEmailSpecification(email);

            // Create test users
            var matchingUser = User.Create("Test User", "test@example.com", "1234567890", "hash");
            var nonMatchingUser = User.Create("Other User", "other@example.com", "0987654321", "hash");

            // Act
            var expression = spec.Criteria;
            var matchingResult = expression.Compile()(matchingUser);
            var nonMatchingResult = expression.Compile()(nonMatchingUser);

            // Assert
            Assert.True(matchingResult);
            Assert.False(nonMatchingResult);
        }

        [Fact]
        public void UserByEmailSpecification_IncludesChurchAndRoles()
        {
            // Arrange
            var email = "test@example.com";
            var spec = new UserByEmailSpecification(email);

            // Act
            var includes = spec.Includes;

            // Assert
            Assert.Contains(includes, include => include.Body.ToString().Contains("Church"));
            Assert.Contains(includes, include => include.Body.ToString().Contains("UserRoles"));
        }

        [Fact]
        public void UserWithRolesSpecification_WithValidUserId_CreatesCorrectExpression()
        {
            // Arrange
            var userId = 123;
            var spec = new UserWithRolesSpecification(userId);

            // Create test users
            var matchingUser = User.Create("Test User", "test@example.com", "1234567890", "hash");
            // Simulate the user having the correct ID (normally set by EF)
            typeof(User).GetProperty("Id")?.SetValue(matchingUser, userId);
            
            var nonMatchingUser = User.Create("Other User", "other@example.com", "0987654321", "hash");
            typeof(User).GetProperty("Id")?.SetValue(nonMatchingUser, 456);

            // Act
            var expression = spec.Criteria;
            var matchingResult = expression.Compile()(matchingUser);
            var nonMatchingResult = expression.Compile()(nonMatchingUser);

            // Assert
            Assert.True(matchingResult);
            Assert.False(nonMatchingResult);
        }

        [Fact]
        public void UserWithRolesSpecification_IncludesChurchAndRoles()
        {
            // Arrange
            var userId = 123;
            var spec = new UserWithRolesSpecification(userId);

            // Act
            var includes = spec.Includes;

            // Assert
            Assert.Contains(includes, include => include.Body.ToString().Contains("Church"));
            Assert.Contains(includes, include => include.Body.ToString().Contains("UserRoles"));
        }

        [Fact]
        public void ActiveUsersSpecification_FiltersVerifiedUsers()
        {
            // Arrange
            var spec = new ActiveUsersSpecification();

            // Create test users
            var verifiedUser = User.Create("Verified User", "verified@example.com", "1234567890", "hash");
            typeof(User).GetProperty("IsVerified")?.SetValue(verifiedUser, true);
            
            var unverifiedUser = User.Create("Unverified User", "unverified@example.com", "0987654321", "hash");
            typeof(User).GetProperty("IsVerified")?.SetValue(unverifiedUser, false);

            // Act
            var expression = spec.Criteria;
            var verifiedResult = expression.Compile()(verifiedUser);
            var unverifiedResult = expression.Compile()(unverifiedUser);

            // Assert
            Assert.True(verifiedResult);
            Assert.False(unverifiedResult);
        }

        [Fact]
        public void ActiveUsersSpecification_IncludesChurchAndOrdersByFullName()
        {
            // Arrange
            var spec = new ActiveUsersSpecification();

            // Act
            var includes = spec.Includes;
            var orderBy = spec.OrderBy;

            // Assert
            Assert.Contains(includes, include => include.Body.ToString().Contains("Church"));
            Assert.NotNull(orderBy);
            Assert.Contains(orderBy.Body.ToString(), "FullName");
        }

        [Fact]
        public void UserByEmailSpecification_WithNullEmail_HandlesGracefully()
        {
            // Arrange & Act
            var spec = new UserByEmailSpecification(null!);
            var expression = spec.Criteria;

            // Create test user
            var user = User.Create("Test User", "test@example.com", "1234567890", "hash");

            // Act
            var result = expression.Compile()(user);

            // Assert
            Assert.False(result); // Should not match when searching for null
        }

        [Fact]
        public void UserByEmailSpecification_WithEmptyEmail_HandlesGracefully()
        {
            // Arrange
            var spec = new UserByEmailSpecification("");
            var expression = spec.Criteria;

            // Create test user
            var user = User.Create("Test User", "test@example.com", "1234567890", "hash");

            // Act
            var result = expression.Compile()(user);

            // Assert
            Assert.False(result); // Should not match when searching for empty string
        }

        [Fact]
        public void UserWithRolesSpecification_WithZeroUserId_CreatesCorrectExpression()
        {
            // Arrange
            var spec = new UserWithRolesSpecification(0);
            var expression = spec.Criteria;

            // Create test user
            var user = User.Create("Test User", "test@example.com", "1234567890", "hash");
            typeof(User).GetProperty("Id")?.SetValue(user, 1);

            // Act
            var result = expression.Compile()(user);

            // Assert
            Assert.False(result); // Should not match user with ID 1 when searching for ID 0
        }
    }
}