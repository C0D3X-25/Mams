using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.products;

namespace Mams_Test.databaseOperations;

public class ResponseGetItemTests
{
    [Fact]
    public void ResponseGetItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var response = new ResponseGetItem<ProductItem>();

        // Assert
        Assert.Null(response.returned_item);
        Assert.Equal(EErrors.NONE, response.error);
        Assert.Null(response.error_message_detail);
        Assert.False(response.is_success);
        Assert.False(response.is_found);
    }

    [Fact]
    public void ResponseGetItem_WithItem_ShouldBeSuccess()
    {
        // Arrange
        var item = new ProductItem { product_id = 1, product_name = "Test" };

        // Act
        var response = new ResponseGetItem<ProductItem>(item);

        // Assert
        Assert.NotNull(response.returned_item);
        Assert.Equal(1, response.returned_item.product_id);
        Assert.True(response.is_success);
        Assert.True(response.is_found);
    }

    [Fact]
    public void ResponseGetItem_Success_ShouldReturnSuccessResponse()
    {
        // Arrange
        var item = new ProductItem { product_id = 1 };

        // Act
        var response = ResponseGetItem<ProductItem>.Success(item);

        // Assert
        Assert.True(response.is_success);
        Assert.True(response.is_found);
        Assert.Equal(EErrors.NONE, response.error);
    }

    [Fact]
    public void ResponseGetItem_NotFound_ShouldReturnNotFoundResponse()
    {
        // Act
        var response = ResponseGetItem<ProductItem>.NotFound();

        // Assert
        Assert.False(response.is_success);
        Assert.False(response.is_found);
        Assert.Null(response.returned_item);
    }

    [Fact]
    public void ResponseGetItem_Failure_ShouldReturnFailureResponse()
    {
        // Act
        var response = ResponseGetItem<ProductItem>.Failure(EErrors.INVALID_INPUT);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
    }

    [Fact]
    public void ResponseGetItem_FailureWithMessage_ShouldReturnFailureWithDetails()
    {
        // Act
        var response = ResponseGetItem<ProductItem>.Failure(EErrors.DATABASE_QUERY, "Test error message");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.DATABASE_QUERY, response.error);
        Assert.Equal("Test error message", response.error_message_detail);
    }

    [Fact]
    public void ResponseGetItem_WithItemAndError_ShouldNotBeSuccess()
    {
        // Arrange
        var item = new ProductItem { product_id = 1 };

        // Act
        var response = new ResponseGetItem<ProductItem>(item, EErrors.UNKNOWN);

        // Assert
        Assert.False(response.is_success);
        Assert.True(response.is_found);
        Assert.Equal(EErrors.UNKNOWN, response.error);
    }
}
