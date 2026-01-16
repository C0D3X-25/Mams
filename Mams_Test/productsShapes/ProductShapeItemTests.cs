using Mams_App.src.productsShapes;

namespace Mams_Test.productsShapes;

public class ProductShapeItemTests
{
    [Fact]
    public void ProductShapeItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ProductShapeItem();

        // Assert
        Assert.Equal(0, item.product_shape_id);
        Assert.Equal(string.Empty, item.product_shape_name);
        Assert.Equal(string.Empty, item.product_shape_archive);
    }

    [Fact]
    public void ProductShapeItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ProductShapeItem();

        // Act
        item.product_shape_id = 1;
        item.product_shape_name = "Test Shape";
        item.product_shape_archive = "2024-01-01";

        // Assert
        Assert.Equal(1, item.product_shape_id);
        Assert.Equal("Test Shape", item.product_shape_name);
        Assert.Equal("2024-01-01", item.product_shape_archive);
    }

    [Fact]
    public void ProductShapeItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new ProductShapeItem { product_shape_name = "Initial Name" };

        // Act
        item.product_shape_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.product_shape_name);
    }

    [Fact]
    public void ProductShapeItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ProductShapeItem();

        // Act
        item.product_shape_id = -1;

        // Assert
        Assert.Equal(-1, item.product_shape_id);
    }
}
