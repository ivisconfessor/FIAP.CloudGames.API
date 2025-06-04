using FIAP.CloudGames.API.Domain.Entities;

namespace FIAP.CloudGames.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void CreateUser_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";
        var password = "Test@123";

        // Act
        var user = new User(name, email, password);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void CreateUser_WithAdminRole_ShouldSetRoleCorrectly()
    {
        // Arrange
        var name = "Admin User";
        var email = "admin@example.com";
        var password = "Admin@123";

        // Act
        var user = new User(name, email, password, UserRole.Admin);

        // Assert
        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void UpdateUser_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = new User("Original Name", "original@example.com", "Password@123");
        var newName = "Updated Name";
        var newEmail = "updated@example.com";

        // Act
        user.Update(newName, newEmail);

        // Assert
        Assert.Equal(newName, user.Name);
        Assert.Equal(newEmail, user.Email);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void ValidatePassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Test@123";
        var user = new User("Test User", "test@example.com", password);

        // Act
        var isValid = user.ValidatePassword(password);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidatePassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = new User("Test User", "test@example.com", "Test@123");

        // Act
        var isValid = user.ValidatePassword("WrongPassword@123");

        // Assert
        Assert.False(isValid);
    }
} 