using Mams_App.src.errors;

namespace Mams_Test.errors;

public class EErrorsTests
{
    [Fact]
    public void EErrors_NONE_ShouldBeZero()
    {
        // Assert
        Assert.Equal(0, (int)EErrors.NONE);
    }

    [Fact]
    public void EErrors_ShouldHaveAllExpectedValues()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.NONE));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.UNKNOWN));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.INVALID_INPUT));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.MISSING_PARAMETER));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.NOT_FOUND));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.ALREADY_EXISTS));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.NULL_VALUE));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.DATABASE_CONNECTION));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.DATABASE_QUERY));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.FOREIGN_KEY_VIOLATION));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.UNIQUE_CONSTRAINT_VIOLATION));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.UNAUTHORIZED));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.TIMEOUT));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.CANCELLED));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.INVALID_OPERATION));
        Assert.True(Enum.IsDefined(typeof(EErrors), EErrors.MISSING_ARCHIVE_FIELD));
    }

    [Fact]
    public void EErrors_AllValues_ShouldBeUnique()
    {
        // Arrange
        var values = Enum.GetValues<EErrors>();

        // Act
        var distinctCount = values.Distinct().Count();

        // Assert
        Assert.Equal(values.Length, distinctCount);
    }

    [Fact]
    public void EErrors_CanBeUsedInSwitch()
    {
        // Arrange
        var error = EErrors.INVALID_INPUT;

        // Act
        var result = error switch
        {
            EErrors.NONE => "Success",
            EErrors.INVALID_INPUT => "Invalid Input",
            EErrors.NOT_FOUND => "Not Found",
            _ => "Other Error"
        };

        // Assert
        Assert.Equal("Invalid Input", result);
    }
}
