using Mams_App.src.users;

namespace Mams_Test.users;

public class UserItemTests
{
    [Fact]
    public void UserItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new UserItem();

        // Assert
        Assert.Equal(0, item.user_id);
        Assert.Equal(string.Empty, item.user_name);
        Assert.Equal(string.Empty, item.user_phone);
        Assert.Equal(string.Empty, item.user_email);
        Assert.Equal(string.Empty, item.user_city);
        Assert.Equal(string.Empty, item.user_address);
    }

    [Fact]
    public void UserItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new UserItem();

        // Act
        item.user_id = 1;
        item.user_name = "Test User";
        item.user_phone = "123-456-7890";
        item.user_email = "test@example.com";
        item.user_city = "Test City";
        item.user_address = "123 Test Street";

        // Assert
        Assert.Equal(1, item.user_id);
        Assert.Equal("Test User", item.user_name);
        Assert.Equal("123-456-7890", item.user_phone);
        Assert.Equal("test@example.com", item.user_email);
        Assert.Equal("Test City", item.user_city);
        Assert.Equal("123 Test Street", item.user_address);
    }

    [Fact]
    public void UserItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new UserItem { user_name = "Initial Name" };

        // Act
        item.user_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.user_name);
    }

    [Fact]
    public void UserItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new UserItem();

        // Act
        item.user_id = -1;

        // Assert
        Assert.Equal(-1, item.user_id);
    }

    [Fact]
    public void UserItem_SetAllContactFields_ShouldAllowEmptyValues()
    {
        // Arrange
        var item = new UserItem
        {
            user_phone = "123-456-7890",
            user_email = "test@example.com",
            user_city = "Test City",
            user_address = "123 Test Street"
        };

        // Act
        item.user_phone = string.Empty;
        item.user_email = string.Empty;
        item.user_city = string.Empty;
        item.user_address = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.user_phone);
        Assert.Equal(string.Empty, item.user_email);
        Assert.Equal(string.Empty, item.user_city);
        Assert.Equal(string.Empty, item.user_address);
    }
}
