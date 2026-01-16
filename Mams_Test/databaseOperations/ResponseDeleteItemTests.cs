using Mams_App.src.databaseOperations;
using Mams_App.src.errors;

namespace Mams_Test.databaseOperations;

public class ResponseDeleteItemTests
{
    [Fact]
    public void ResponseDeleteItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var response = new ResponseDeleteItem();

        // Assert
        Assert.False(response.is_deleted);
        Assert.Equal(EErrors.NONE, response.error);
        Assert.Null(response.error_message_detail);
        Assert.False(response.is_success);
    }

    [Fact]
    public void ResponseDeleteItem_WithDeleted_ShouldBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseDeleteItem(true);

        // Assert
        Assert.True(response.is_deleted);
        Assert.True(response.is_success);
    }

    [Fact]
    public void ResponseDeleteItem_WithNotDeleted_ShouldNotBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseDeleteItem(false);

        // Assert
        Assert.False(response.is_deleted);
        Assert.False(response.is_success);
    }

    [Fact]
    public void ResponseDeleteItem_Success_ShouldReturnSuccessResponse()
    {
        // Act
        var response = ResponseDeleteItem.Success();

        // Assert
        Assert.True(response.is_success);
        Assert.True(response.is_deleted);
        Assert.Equal(EErrors.NONE, response.error);
    }

    [Fact]
    public void ResponseDeleteItem_Failure_ShouldReturnFailureResponse()
    {
        // Act
        var response = ResponseDeleteItem.Failure(EErrors.NOT_FOUND);

        // Assert
        Assert.False(response.is_success);
        Assert.False(response.is_deleted);
        Assert.Equal(EErrors.NOT_FOUND, response.error);
    }

    [Fact]
    public void ResponseDeleteItem_FailureWithMessage_ShouldReturnFailureWithDetails()
    {
        // Act
        var response = ResponseDeleteItem.Failure(EErrors.DATABASE_QUERY, "Test error message");

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.DATABASE_QUERY, response.error);
        Assert.Equal("Test error message", response.error_message_detail);
    }

    [Fact]
    public void ResponseDeleteItem_WithDeletedAndError_ShouldNotBeSuccess()
    {
        // Arrange & Act
        var response = new ResponseDeleteItem(true, EErrors.UNKNOWN);

        // Assert
        Assert.False(response.is_success);
        Assert.True(response.is_deleted);
        Assert.Equal(EErrors.UNKNOWN, response.error);
    }

    [Fact]
    public void ResponseDeleteItem_WithAllValues_ShouldHaveAllValues()
    {
        // Arrange & Act
        var response = new ResponseDeleteItem(false, EErrors.FOREIGN_KEY_VIOLATION, "Cannot delete due to foreign key");

        // Assert
        Assert.False(response.is_success);
        Assert.False(response.is_deleted);
        Assert.Equal(EErrors.FOREIGN_KEY_VIOLATION, response.error);
        Assert.Equal("Cannot delete due to foreign key", response.error_message_detail);
    }
}
