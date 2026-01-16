using Mams_App.src.suppliers;
using Mams_App.src.errors;

namespace Mams_Test.suppliers;

public class SupplierModelTests
{
    [Fact]
    public void GetItemByID_InvalidId_ReturnsFailure()
    {
        // Arrange
        var model = new SupplierModel();

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
        var model = new SupplierModel();

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
        var model = new SupplierModel();

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
        var model = new SupplierModel();

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
        var model = new SupplierModel();

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
        var model = new SupplierModel();

        // Act
        var response = model.saveItem(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.NULL_VALUE, response.error);
        Assert.Contains("cannot be null", response.error_message_detail);
        Assert.Equal(0, response.returned_id);
    }

    [Fact]
    public void GetSupplierWithEntityFK_InvalidId_ReturnsNull()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var result = model.getSupplierWithEntityFK("invalid-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetSupplierWithEntityFK_EmptyId_ReturnsNull()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var result = model.getSupplierWithEntityFK(string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetSupplierWithEntityFK_NullId_ReturnsNull()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var result = model.getSupplierWithEntityFK(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeleteSupplierWithEntityFK_InvalidId_ReturnsFailure()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var response = model.deleteSupplierWithEntityFK("invalid-id");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Contains("Invalid FK entity ID provided", response.error_message_detail);
    }

    [Fact]
    public void DeleteSupplierWithEntityFK_EmptyId_ReturnsFailure()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var response = model.deleteSupplierWithEntityFK(string.Empty);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
    }

    [Fact]
    public void DeleteSupplierWithEntityFK_NullId_ReturnsFailure()
    {
        // Arrange
        var model = new SupplierModel();

        // Act
        var response = model.deleteSupplierWithEntityFK(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
    }
}
