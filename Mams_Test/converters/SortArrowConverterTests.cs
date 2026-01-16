using Mams_App.src.converters;
using System.ComponentModel;
using System.Windows;

namespace Mams_Test.converters;

public class SortArrowConverterTests
{
    private readonly SortArrowConverter _converter = new();

    [Fact]
    public void Convert_MatchingColumnAscending_ReturnsUpArrow()
    {
        // Arrange
        object[] values = ["column1", "column1", ListSortDirection.Ascending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(" \u2191", result);
    }

    [Fact]
    public void Convert_MatchingColumnDescending_ReturnsDownArrow()
    {
        // Arrange
        object[] values = ["column1", "column1", ListSortDirection.Descending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(" \u2193", result);
    }

    [Fact]
    public void Convert_DifferentColumns_ReturnsEmptyString()
    {
        // Arrange
        object[] values = ["column1", "column2", ListSortDirection.Ascending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_NullCurrentColumn_ReturnsEmptyString()
    {
        // Arrange
        object[] values = [null!, "column2", ListSortDirection.Ascending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_EmptyCurrentColumn_ReturnsEmptyString()
    {
        // Arrange
        object[] values = [string.Empty, "column2", ListSortDirection.Ascending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_LessThanThreeValues_ReturnsEmptyString()
    {
        // Arrange
        object[] values = ["column1", "column1"];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_UnsetValue_ReturnsEmptyString()
    {
        // Arrange
        object[] values = [DependencyProperty.UnsetValue, "column1", ListSortDirection.Ascending];

        // Act
        var result = _converter.Convert(values, typeof(string), null!, null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConvertBack_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("test", [typeof(string)], null!, null!));
    }
}
