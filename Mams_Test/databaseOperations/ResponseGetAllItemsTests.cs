using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.products;
using System.Collections.ObjectModel;

namespace Mams_Test.databaseOperations;

public class ResponseGetAllItemsTests
{
    [Fact]
    public void ResponseGetAllItems_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var response = new ResponseGetAllItems<ProductItem>();

        // Assert
        Assert.NotNull(response.returned_items);
        Assert.Empty(response.returned_items);
        Assert.Equal(EErrors.NONE, response.error);
        Assert.Null(response.error_message_detail);
        Assert.True(response.is_success);
        Assert.False(response.has_items);
        Assert.Equal(0, response.count);
    }

    [Fact]
    public void ResponseGetAllItems_WithItems_ShouldHaveItems()
    {
        // Arrange
        var items = new ObservableCollection<ProductItem>
        {
            new ProductItem { product_id = 1 },
            new ProductItem { product_id = 2 }
        };

        // Act
        var response = new ResponseGetAllItems<ProductItem>(items);

        // Assert
        Assert.True(response.is_success);
        Assert.True(response.has_items);
        Assert.Equal(2, response.count);
    }

    [Fact]
    public void ResponseGetAllItems_WithEmptyCollection_ShouldNotHaveItems()
    {
        // Arrange
        var items = new ObservableCollection<ProductItem>();

        // Act
        var response = new ResponseGetAllItems<ProductItem>(items);

        // Assert
        Assert.True(response.is_success);
        Assert.False(response.has_items);
        Assert.Equal(0, response.count);
    }

    [Fact]
    public void ResponseGetAllItems_SetError_ShouldNotBeSuccess()
    {
        // Arrange
        var response = new ResponseGetAllItems<ProductItem>();

        // Act
        response.error = EErrors.DATABASE_QUERY;

        // Assert
        Assert.False(response.is_success);
    }

    [Fact]
    public void ResponseGetAllItems_SetItems_ShouldUpdateCollection()
    {
        // Arrange
        var response = new ResponseGetAllItems<ProductItem>();
        var items = new ObservableCollection<ProductItem>
        {
            new ProductItem { product_id = 1 }
        };

        // Act
        response.returned_items = items;

        // Assert
        Assert.True(response.has_items);
        Assert.Equal(1, response.count);
    }

    [Fact]
    public void ResponseGetAllItems_SetErrorMessageDetail_ShouldHaveDetail()
    {
        // Arrange
        var response = new ResponseGetAllItems<ProductItem>();

        // Act
        response.error = EErrors.DATABASE_QUERY;
        response.error_message_detail = "Query failed";

        // Assert
        Assert.Equal("Query failed", response.error_message_detail);
    }
}
