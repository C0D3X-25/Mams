using Mams_App.src.helpers;

namespace Mams_Test.helpers;

public class SFormatDataTests
{
    #region formatEUDateToMySQLDate

    [Fact]
    public void FormatEUDateToMySQLDate_ValidDate_ReturnsCorrectFormat()
    {
        // Act
        var result = SFormatData.formatEUDateToMySQLDate("01.01.2024");

        // Assert
        Assert.Equal("2024-01-01", result);
    }

    [Fact]
    public void FormatEUDateToMySQLDate_EmptyString_ReturnsEmptyString()
    {
        // Act
        var result = SFormatData.formatEUDateToMySQLDate(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatEUDateToMySQLDate_NullString_ReturnsEmptyString()
    {
        // Act
        var result = SFormatData.formatEUDateToMySQLDate(null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatEUDateToMySQLDate_DifferentDates_ReturnsCorrectFormat()
    {
        // Act & Assert
        Assert.Equal("2024-12-31", SFormatData.formatEUDateToMySQLDate("31.12.2024"));
        Assert.Equal("2023-06-15", SFormatData.formatEUDateToMySQLDate("15.06.2023"));
        Assert.Equal("2000-01-01", SFormatData.formatEUDateToMySQLDate("01.01.2000"));
    }

    #endregion

    #region getYearFromDate

    [Fact]
    public void GetYearFromDate_ValidEUDate_ReturnsYear()
    {
        // Act
        var result = SFormatData.getYearFromDate("01.01.2024");

        // Assert
        Assert.Equal("2024", result);
    }

    [Fact]
    public void GetYearFromDate_FourDigitYear_ReturnsSameYear()
    {
        // Act
        var result = SFormatData.getYearFromDate("2024");

        // Assert
        Assert.Equal("2024", result);
    }

    [Fact]
    public void GetYearFromDate_EmptyString_ReturnsEmptyString()
    {
        // Act
        var result = SFormatData.getYearFromDate(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetYearFromDate_DifferentDates_ReturnsCorrectYears()
    {
        // Act & Assert
        Assert.Equal("2023", SFormatData.getYearFromDate("15.06.2023"));
        Assert.Equal("2000", SFormatData.getYearFromDate("01.01.2000"));
        Assert.Equal("1999", SFormatData.getYearFromDate("31.12.1999"));
    }

    #endregion
}
