using Mams_App.src.helpers;

namespace Mams_Test.helpers;

public class SDataValidationTests
{
    #region isIdValid (string)

    [Fact]
    public void IsIdValid_String_ValidPositiveId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValid("1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValid_String_ZeroId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValid("0");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValid_String_NegativeId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid("-1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValid_String_EmptyString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid(string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValid_String_NullString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid((string)null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValid_String_NonNumericString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid("abc");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValid_String_LargeNumber_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValid("999999999");

        // Assert
        Assert.True(result);
    }

    #endregion

    #region isIdValidForRetrieval (string)

    [Fact]
    public void IsIdValidForRetrieval_String_ValidPositiveId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval("1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_String_ZeroId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval("0");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_String_NegativeId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval("-1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_String_EmptyString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval(string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_String_NullString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval((string)null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_String_NonNumericString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval("abc");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isIdValid (int?)

    [Fact]
    public void IsIdValid_Int_ValidPositiveId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValid(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValid_Int_ZeroId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValid(0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValid_Int_NegativeId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValid_Int_NullId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValid((int?)null);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isIdValidForRetrieval (int?)

    [Fact]
    public void IsIdValidForRetrieval_Int_ValidPositiveId_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_Int_ZeroId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval(0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_Int_NegativeId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIdValidForRetrieval_Int_NullId_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isIdValidForRetrieval((int?)null);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isInteger

    [Fact]
    public void IsInteger_ValidInteger_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isInteger("123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInteger_NegativeInteger_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isInteger("-123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInteger_Zero_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isInteger("0");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInteger_NonNumeric_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isInteger("abc");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInteger_DecimalNumber_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isInteger("12.5");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInteger_EmptyString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isInteger(string.Empty);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isPositiveInteger

    [Fact]
    public void IsPositiveInteger_ValidPositive_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isPositiveInteger("123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPositiveInteger_Zero_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isPositiveInteger("0");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPositiveInteger_NegativeNumber_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isPositiveInteger("-123");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPositiveInteger_NonNumeric_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isPositiveInteger("abc");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isYearInRange

    [Fact]
    public void IsYearInRange_ValidYear_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isYearInRange(2024);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsYearInRange_YearBefore1900_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isYearInRange(1899);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsYearInRange_Year1900_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isYearInRange(1900);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsYearInRange_WithCustomMinMax_ValidYear_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isYearInRange(2020, 2000, 2030);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsYearInRange_WithCustomMinMax_YearBelowMin_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isYearInRange(1999, 2000, 2030);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsYearInRange_WithCustomMinMax_YearAboveMax_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isYearInRange(2031, 2000, 2030);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsYearInRange_NegativeMin_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isYearInRange(2020, -1, 2030);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsYearInRange_NegativeMax_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isYearInRange(2020, 2000, -1);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region isDateValidFormatEU

    [Fact]
    public void IsDateValidFormatEU_ValidDate_ReturnsTrue()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU("01.01.2024");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDateValidFormatEU_EmptyString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU(string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDateValidFormatEU_NullString_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDateValidFormatEU_WhitespaceOnly_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU("   ");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDateValidFormatEU_InvalidFormat_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU("2024-01-01");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDateValidFormatEU_InvalidDate_ReturnsFalse()
    {
        // Act
        var result = SDataValidation.isDateValidFormatEU("32.13.2024");

        // Assert
        Assert.False(result);
    }

    #endregion
}
