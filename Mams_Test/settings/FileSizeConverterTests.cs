using Mams_App.src.settings;

namespace Mams_Test.settings;

public class FileSizeConverterTests
{
    private readonly FileSizeConverter _converter = new();

    [Fact]
    public void Convert_ZeroBytes_ReturnsZeroB()
    {
        // Act
        var result = _converter.Convert(0L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("0.0 B", result);
    }

    [Fact]
    public void Convert_BytesUnderKB_ReturnsCorrectBytes()
    {
        // Act
        var result = _converter.Convert(512L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("512.0 B", result);
    }

    [Fact]
    public void Convert_OneKB_ReturnsCorrectKB()
    {
        // Act
        var result = _converter.Convert(1024L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("1.0 KB", result);
    }

    [Fact]
    public void Convert_OneMB_ReturnsCorrectMB()
    {
        // Act
        var result = _converter.Convert(1048576L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("1.0 MB", result);
    }

    [Fact]
    public void Convert_OneGB_ReturnsCorrectGB()
    {
        // Act
        var result = _converter.Convert(1073741824L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("1.0 GB", result);
    }

    [Fact]
    public void Convert_LargeGBValue_ReturnsCorrectGB()
    {
        // Act
        var result = _converter.Convert(5368709120L, typeof(string), null!, null!);

        // Assert
        Assert.Equal("5.0 GB", result);
    }

    [Fact]
    public void Convert_FractionalKB_ReturnsCorrectFormat()
    {
        // Arrange
        long bytes = 1536L; // 1.5 KB

        // Act
        var result = _converter.Convert(bytes, typeof(string), null!, null!);

        // Assert
        Assert.Equal("1.5 KB", result);
    }

    [Fact]
    public void Convert_FractionalMB_ReturnsCorrectFormat()
    {
        // Arrange
        long bytes = 2621440L; // 2.5 MB

        // Act
        var result = _converter.Convert(bytes, typeof(string), null!, null!);

        // Assert
        Assert.Equal("2.5 MB", result);
    }

    [Fact]
    public void Convert_NonLongValue_ReturnsZeroB()
    {
        // Act
        var result = _converter.Convert("not a long", typeof(string), null!, null!);

        // Assert
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsZeroB()
    {
        // Act
        var result = _converter.Convert(null!, typeof(string), null!, null!);

        // Assert
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void Convert_IntValue_ReturnsZeroB()
    {
        // Act - int is not long
        var result = _converter.Convert(1024, typeof(string), null!, null!);

        // Assert
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void ConvertBack_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("1.0 KB", typeof(long), null!, null!));
    }
}
