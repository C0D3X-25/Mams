using Mams_App.src.search;
using Mams_App.src.databaseOperations;

namespace Mams_Test.search;

public class SearchItemTests
{
    [Fact]
    public void SearchItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new SearchItem();

        // Assert
        Assert.Equal(0, item.search_id);
        Assert.Equal(EDatabaseTableName.NONE, item.search_table);
        Assert.Equal(string.Empty, item.search_item_to_display);
        Assert.Equal(string.Empty, item.search_year);
    }

    [Fact]
    public void SearchItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new SearchItem();

        // Act
        item.search_id = 1;
        item.search_table = EDatabaseTableName.PRODUCT;
        item.search_item_to_display = "Test Product";
        item.search_year = "2024";

        // Assert
        Assert.Equal(1, item.search_id);
        Assert.Equal(EDatabaseTableName.PRODUCT, item.search_table);
        Assert.Equal("Test Product", item.search_item_to_display);
        Assert.Equal("2024", item.search_year);
    }

    [Fact]
    public void SearchItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new SearchItem();

        // Act
        item.search_id = -1;

        // Assert
        Assert.Equal(-1, item.search_id);
    }

    [Fact]
    public void SearchItem_SetEmptyDisplayText_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new SearchItem { search_item_to_display = "Initial Value" };

        // Act
        item.search_item_to_display = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.search_item_to_display);
    }

    [Fact]
    public void SearchItem_SetDifferentTableTypes_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new SearchItem();

        // Act & Assert - Test different table types
        item.search_table = EDatabaseTableName.ENTITY;
        Assert.Equal(EDatabaseTableName.ENTITY, item.search_table);

        item.search_table = EDatabaseTableName.BEEHIVE;
        Assert.Equal(EDatabaseTableName.BEEHIVE, item.search_table);

        item.search_table = EDatabaseTableName.PRODUCT_CATEGORY;
        Assert.Equal(EDatabaseTableName.PRODUCT_CATEGORY, item.search_table);
    }

    [Fact]
    public void SearchItem_SetEmptyYear_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new SearchItem { search_year = "2024" };

        // Act
        item.search_year = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.search_year);
    }
}
