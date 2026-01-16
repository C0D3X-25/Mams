using Mams_App.src.databaseOperations;
using Mams_App.src.errors;

namespace Mams_Test.databaseOperations;

public class ResponseSaveItemTests
{
    [Fact]
    public void ResponseSaveItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var response = new ResponseSaveItem();

        // Assert
        Assert.Equal(0, response.returned_id);
        Assert.Equal(EErrors.NONE, response.error);
        Assert.Null(response.error_message_detail);
        Assert.False(response.is_success);
    }

    [Fact]
    public void ResponseSaveItem_WithValidId_ShouldBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseSaveItem(1);

        // Assert
        Assert.Equal(1, response.returned_id);
        Assert.True(response.is_success);
    }

    [Fact]
    public void ResponseSaveItem_WithZeroId_ShouldNotBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseSaveItem(0);

        // Assert
        Assert.Equal(0, response.returned_id);
        Assert.False(response.is_success);
    }

    [Fact]
    public void ResponseSaveItem_Success_ShouldReturnSuccessResponse()
    {
        // Act
        var response = ResponseSaveItem.Success(5);

        // Assert
        Assert.True(response.is_success);
        Assert.Equal(5, response.returned_id);
        Assert.Equal(EErrors.NONE, response.error);
    }

    [Fact]
    public void ResponseSaveItem_Failure_ShouldReturnFailureResponse()
    {
        // Act
        var response = ResponseSaveItem.Failure(EErrors.NULL_VALUE);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(0, response.returned_id);
        Assert.Equal(EErrors.NULL_VALUE, response.error);
    }

    [Fact]
    public void ResponseSaveItem_FailureWithMessage_ShouldReturnFailureWithDetails()
    {
        // Act
        var response = ResponseSaveItem.Failure(EErrors.DATABASE_QUERY, "Test error message");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.DATABASE_QUERY, response.error);
        Assert.Equal("Test error message", response.error_message_detail);
    }

    [Fact]
    public void ResponseSaveItem_WithIdAndError_ShouldNotBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseSaveItem(1, EErrors.UNKNOWN);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(1, response.returned_id);
        Assert.Equal(EErrors.UNKNOWN, response.error);
    }

    [Fact]
    public void ResponseSaveItem_WithIdErrorAndMessage_ShouldHaveAllValues()
    {
        // Arrange & Act
        var response = new ResponseSaveItem(0, EErrors.INVALID_INPUT, "Invalid data provided");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(0, response.returned_id);
        Assert.Equal(EErrors.INVALID_INPUT, response.error);
        Assert.Equal("Invalid data provided", response.error_message_detail);
    }
}
