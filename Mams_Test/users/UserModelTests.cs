using Mams_App.src.users;
using Mams_App.src.errors;

namespace Mams_Test.users;

public class UserModelTests
{
    [Fact]
    public void SaveUser_NullItem_ReturnsFailure()
    {
        // Arrange
        var model = new UserModel();

        // Act
        var response = model.saveUser(null!);

        // Assert
        Assert.False(response.is_success);
        Assert.Equal(EErrors.NULL_VALUE, response.error);
        Assert.Contains("cannot be null", response.error_message_detail);
        Assert.Equal(0, response.returned_id);
    }
}
