using Mams_App.src.converters;

namespace Mams_Test.converters;

public class BooleanToColumnWidthConverterTests
{
    private readonly BooleanToColumnWidthConverter _converter = new();

    [Fact]
    public void Convert_TrueValue_ReturnsNaN()
    {
        // Act
        var result = _converter.Convert(true, typeof(double), null!, null!);

        // Assert
        Assert.Equal(double.NaN, result);
    }

    [Fact]
    public void Convert_FalseValue_ReturnsZero()
    {
        // Act
        var result = _converter.Convert(false, typeof(double), null!, null!);

        // Assert
        Assert.Equal(0d, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsZero()
    {
        // Act
        var result = _converter.Convert(null!, typeof(double), null!, null!);

        // Assert
        Assert.Equal(0d, result);
    }

    [Fact]
    public void Convert_NonBooleanValue_ReturnsZero()
    {
        // Act
        var result = _converter.Convert("not a boolean", typeof(double), null!, null!);

        // Assert
        Assert.Equal(0d, result);
    }

    [Fact]
    public void Convert_IntegerValue_ReturnsZero()
    {
        // Act
        var result = _converter.Convert(1, typeof(double), null!, null!);

        // Assert
        Assert.Equal(0d, result);
    }

    [Fact]
    public void ConvertBack_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(0d, typeof(bool), null!, null!));
    }
}
