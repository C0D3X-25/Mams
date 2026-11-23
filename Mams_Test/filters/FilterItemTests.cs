using Mams_App.src.filters;
using Mams_App.src.databaseOperations;

namespace Mams_Test.filters;

public class FilterItemTests
{
    [Fact]
    public void FilterItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new FilterItem();

        // Assert
        Assert.Equal(0, item.filter_id);
        Assert.Equal(EDatabaseTableName.NONE, item.filter_table);
        Assert.Equal(string.Empty, item.filter_item_to_display);
        Assert.Equal(string.Empty, item.filter_year);
    }

    [Fact]
    public void FilterItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new FilterItem();

        // Act
        item.filter_id = 1;
        item.filter_table = EDatabaseTableName.PRODUCT;
        item.filter_item_to_display = "Test Product";
        item.filter_year = "2024";

        // Assert
        Assert.Equal(1, item.filter_id);
        Assert.Equal(EDatabaseTableName.PRODUCT, item.filter_table);
        Assert.Equal("Test Product", item.filter_item_to_display);
        Assert.Equal("2024", item.filter_year);
    }

    [Fact]
    public void FilterItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new FilterItem();

        // Act
        item.filter_id = -1;

        // Assert
        Assert.Equal(-1, item.filter_id);
    }

    [Fact]
    public void FilterItem_SetEmptyDisplayText_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new FilterItem { filter_item_to_display = "Initial Value" };

        // Act
        item.filter_item_to_display = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.filter_item_to_display);
    }

    [Fact]
    public void FilterItem_SetDifferentTableTypes_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new FilterItem();

        // Act & Assert - Test different table types
        item.filter_table = EDatabaseTableName.ENTITY;
        Assert.Equal(EDatabaseTableName.ENTITY, item.filter_table);

        item.filter_table = EDatabaseTableName.BEEHIVE;
        Assert.Equal(EDatabaseTableName.BEEHIVE, item.filter_table);

        item.filter_table = EDatabaseTableName.PRODUCT_CATEGORY;
        Assert.Equal(EDatabaseTableName.PRODUCT_CATEGORY, item.filter_table);
    }
}
