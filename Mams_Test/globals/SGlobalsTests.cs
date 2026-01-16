using Mams_App.src.globals;

namespace Mams_Test.globals;

public class SGlobalsTests
{
    [Fact]
    public void EUDateFormat_ShouldBeCorrectFormat()
    {
        // Assert
        Assert.Equal("dd.MM.yyyy", SGlobals.g_EU_DATE_FORMAT);
    }

    [Fact]
    public void EUDateFormat_ShouldParseValidDates()
    {
        // Arrange
        var testDate = "25.12.2024";

        // Act
        var result = DateTime.ParseExact(testDate, SGlobals.g_EU_DATE_FORMAT, null);

        // Assert
        Assert.Equal(25, result.Day);
        Assert.Equal(12, result.Month);
        Assert.Equal(2024, result.Year);
    }

    [Fact]
    public void EUDateFormat_ShouldFormatDatesCorrectly()
    {
        // Arrange
        var date = new DateTime(2024, 12, 25);

        // Act
        var result = date.ToString(SGlobals.g_EU_DATE_FORMAT);

        // Assert
        Assert.Equal("25.12.2024", result);
    }
}
