using TaskManagementApi.Models;
using Xunit;

namespace TaskManagementApi.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_ShouldInitializeWithDefaultValues()
    {
        // Act
        var user = new User();

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void User_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";
        var email = "test@example.com";
        var password = "hashedpassword";

        // Act
        var user = new User
        {
            Id = userId,
            Username = username,
            Email = email,
            Password = password
        };

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
    }

    [Theory]
    [InlineData("user1", "user1@example.com")]
    [InlineData("admin", "admin@company.com")]
    [InlineData("test_user", "test.user@domain.org")]
    public void User_ShouldAcceptValidUsernameAndEmail(string username, string email)
    {
        // Act
        var user = new User
        {
            Username = username,
            Email = email,
            Password = "password123"
        };

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void User_ShouldHaveUniqueIds()
    {
        // Act
        var user1 = new User();
        var user2 = new User();

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void User_CreatedAt_ShouldBeSetToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var user = new User();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(user.CreatedAt >= beforeCreation);
        Assert.True(user.CreatedAt <= afterCreation);
    }
}

