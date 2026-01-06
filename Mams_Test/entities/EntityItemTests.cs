using Mams_App.src.entities;

namespace MamsTest.entities;

public class EntityItemTests
{
    [Fact]
    public void EntityItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new EntityItem();

        // Assert
        Assert.Equal(0, item.entity_id);
        Assert.Equal(string.Empty, item.entity_name);
        Assert.Equal(string.Empty, item.entity_phone);
        Assert.Equal(string.Empty, item.entity_email);
        Assert.Equal(string.Empty, item.entity_city);
        Assert.Equal(string.Empty, item.entity_address);
        Assert.Equal(string.Empty, item.entity_archive);
    }

    [Fact]
    public void EntityItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new EntityItem();

        // Act
        item.entity_id = 1;
        item.entity_name = "Test Entity";
        item.entity_phone = "123-456-7890";
        item.entity_email = "test@example.com";
        item.entity_city = "Test City";
        item.entity_address = "123 Test Street";
        item.entity_archive = "2024-01-01";

        // Assert
        Assert.Equal(1, item.entity_id);
        Assert.Equal("Test Entity", item.entity_name);
        Assert.Equal("123-456-7890", item.entity_phone);
        Assert.Equal("test@example.com", item.entity_email);
        Assert.Equal("Test City", item.entity_city);
        Assert.Equal("123 Test Street", item.entity_address);
        Assert.Equal("2024-01-01", item.entity_archive);
    }

    [Fact]
    public void EntityItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new EntityItem { entity_name = "Initial Name" };

        // Act
        item.entity_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.entity_name);
    }

    [Fact]
    public void EntityItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new EntityItem();

        // Act
        item.entity_id = -1;

        // Assert
        Assert.Equal(-1, item.entity_id);
    }

    [Fact]
    public void EntityItem_SetAllContactFields_ShouldAllowEmptyValues()
    {
        // Arrange
        var item = new EntityItem
        {
            entity_phone = "123-456-7890",
            entity_email = "test@example.com",
            entity_city = "Test City",
            entity_address = "123 Test Street"
        };

        // Act
        item.entity_phone = string.Empty;
        item.entity_email = string.Empty;
        item.entity_city = string.Empty;
        item.entity_address = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.entity_phone);
        Assert.Equal(string.Empty, item.entity_email);
        Assert.Equal(string.Empty, item.entity_city);
        Assert.Equal(string.Empty, item.entity_address);
    }
}
