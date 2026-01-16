using Mams_App.src.products;

namespace Mams_Test.products;

public class ProductItemTests
{
    [Fact]
    public void ProductItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ProductItem();

        // Assert
        Assert.Equal(0, item.product_id);
        Assert.Equal(string.Empty, item.product_name);
        Assert.Equal(0, item.product_weight);
        Assert.Equal(string.Empty, item.product_archive);
        Assert.Equal(0, item.fk_product_type_id);
        Assert.Equal(string.Empty, item.product_type_name);
        Assert.Equal(0, item.fk_product_category_id);
        Assert.Equal(string.Empty, item.product_category_name);
        Assert.Equal(0, item.fk_product_shape_id);
        Assert.Equal(string.Empty, item.product_shape_name);
    }

    [Fact]
    public void ProductItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ProductItem();

        // Act
        item.product_id = 1;
        item.product_name = "Test Product";
        item.product_weight = 500;
        item.product_archive = "2024-01-01";
        item.fk_product_type_id = 2;
        item.product_type_name = "Type A";
        item.fk_product_category_id = 3;
        item.product_category_name = "Category B";
        item.fk_product_shape_id = 4;
        item.product_shape_name = "Shape C";

        // Assert
        Assert.Equal(1, item.product_id);
        Assert.Equal("Test Product", item.product_name);
        Assert.Equal(500, item.product_weight);
        Assert.Equal("2024-01-01", item.product_archive);
        Assert.Equal(2, item.fk_product_type_id);
        Assert.Equal("Type A", item.product_type_name);
        Assert.Equal(3, item.fk_product_category_id);
        Assert.Equal("Category B", item.product_category_name);
        Assert.Equal(4, item.fk_product_shape_id);
        Assert.Equal("Shape C", item.product_shape_name);
    }

    [Fact]
    public void ProductItem_SetZeroWeight_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ProductItem { product_weight = 100 };

        // Act
        item.product_weight = 0;

        // Assert
        Assert.Equal(0, item.product_weight);
    }

    [Fact]
    public void ProductItem_SetNegativeWeight_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ProductItem();

        // Act
        item.product_weight = -10;

        // Assert
        Assert.Equal(-10, item.product_weight);
    }

    [Fact]
    public void ProductItem_SetForeignKeys_ShouldAllowZeroValues()
    {
        // Arrange
        var item = new ProductItem
        {
            fk_product_type_id = 1,
            fk_product_category_id = 2,
            fk_product_shape_id = 3
        };

        // Act
        item.fk_product_type_id = 0;
        item.fk_product_category_id = 0;
        item.fk_product_shape_id = 0;

        // Assert
        Assert.Equal(0, item.fk_product_type_id);
        Assert.Equal(0, item.fk_product_category_id);
        Assert.Equal(0, item.fk_product_shape_id);
    }
}
