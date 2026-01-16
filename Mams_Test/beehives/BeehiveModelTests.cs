using Mams_App.src.beehives;
using Mams_App.src.errors;

namespace Mams_Test.beehives;

public class BeehiveModelTests
{
    [Fact]
    public void GetItemByID_InvalidId_ReturnsFailure()
    {
        // Arrange
        var model = new BeehiveModel();

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
        var model = new BeehiveModel();

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
        var model = new BeehiveModel();

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
        var model = new BeehiveModel();

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
        var model = new BeehiveModel();

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
        var model = new BeehiveModel();

        // Act
        var response = model.saveItem(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.NULL_VALUE, response.error);
        Assert.Contains("cannot be null", response.error_message_detail);
        Assert.Equal(0, response.returned_id);
    }
}
