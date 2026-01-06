using Mams_App.src.products;
using Mams_App.src.errors;

namespace MamsTest.products;

public class ProductModelTests
{
    [Fact]
    public void GetItemByID_InvalidId_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.getItemByID("invalid-id");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Contains("Invalid ID provided", response.error_message_detail);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void GetItemByID_EmptyId_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.getItemByID(string.Empty);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void GetItemByID_NullId_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.getItemByID(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void GetItemByID_NegativeId_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.getItemByID("-1");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void GetItemByID_ZeroId_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.getItemByID("0");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void SaveItem_NullItem_ReturnsFailure()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var response = model.saveItem(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.NULL_VALUE, response.error);
        Assert.Contains("cannot be null", response.error_message_detail);
        Assert.Equal(0, response.returned_id);
    }

    [Fact]
    public void GetProductWithProductTypeId_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var result = model.getProductWithProductTypeId([]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetProductWithProductCategoryId_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var result = model.getProductWithProductCategoryId([]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetProductWithProductShapeId_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var result = model.getProductWithProductShapeId([]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetProductWithProductTypeId_InvalidIds_ReturnsEmptyCollection()
    {
        // Arrange
        var model = new ProductModel();

        // Act
        var result = model.getProductWithProductTypeId([-1, 0]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
