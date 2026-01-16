using Mams_App.src.productsTypes;

namespace Mams_Test.productsTypes;

public class ProductTypeItemTests
{
    [Fact]
    public void ProductTypeItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ProductTypeItem();

        // Assert
        Assert.Equal(0, item.product_type_id);
        Assert.Equal(string.Empty, item.product_type_name);
        Assert.Equal(string.Empty, item.product_type_archive);
    }

    [Fact]
    public void ProductTypeItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ProductTypeItem();

        // Act
        item.product_type_id = 1;
        item.product_type_name = "Test Type";
        item.product_type_archive = "2024-01-01";

        // Assert
        Assert.Equal(1, item.product_type_id);
        Assert.Equal("Test Type", item.product_type_name);
        Assert.Equal("2024-01-01", item.product_type_archive);
    }

    [Fact]
    public void ProductTypeItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new ProductTypeItem { product_type_name = "Initial Name" };

        // Act
        item.product_type_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.product_type_name);
    }

    [Fact]
    public void ProductTypeItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ProductTypeItem();

        // Act
        item.product_type_id = -1;

        // Assert
        Assert.Equal(-1, item.product_type_id);
    }
}
