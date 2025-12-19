using Mams.src.productsCategories;

namespace MamsTest.productsCategories;

public class ProductCategoryItemTests
{
    [Fact]
    public void ProductCategoryItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ProductCategoryItem();

        // Assert
        Assert.Equal(0, item.product_category_id);
        Assert.Equal(string.Empty, item.product_category_name);
        Assert.Equal(string.Empty, item.product_category_archive);
    }

    [Fact]
    public void ProductCategoryItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ProductCategoryItem();

        // Act
        item.product_category_id = 1;
        item.product_category_name = "Test Category";
        item.product_category_archive = "2024-01-01";

        // Assert
        Assert.Equal(1, item.product_category_id);
        Assert.Equal("Test Category", item.product_category_name);
        Assert.Equal("2024-01-01", item.product_category_archive);
    }

    [Fact]
    public void ProductCategoryItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new ProductCategoryItem { product_category_name = "Initial Name" };

        // Act
        item.product_category_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.product_category_name);
    }

    [Fact]
    public void ProductCategoryItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ProductCategoryItem();

        // Act
        item.product_category_id = -1;

        // Assert
        Assert.Equal(-1, item.product_category_id);
    }
}
