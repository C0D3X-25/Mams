using Mams_App.src.configurations;

namespace Mams_Test.configurations;

public class AppConfigItemTests
{
    [Fact]
    public void AppConfigItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new AppConfigItem();

        // Assert
        Assert.NotNull(item.m_window);
        Assert.NotNull(item.m_localization);
    }

    [Fact]
    public void AppConfigItem_SetWindow_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new AppConfigItem();
        var window = new WindowConfigItem { m_width = 1920, m_height = 1080 };

        // Act
        item.m_window = window;

        // Assert
        Assert.Equal(1920, item.m_window.m_width);
        Assert.Equal(1080, item.m_window.m_height);
    }

    [Fact]
    public void AppConfigItem_SetLocalization_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new AppConfigItem();
        var localization = new LocalizationConfigItem { m_language = "en", m_culture = "en-US" };

        // Act
        item.m_localization = localization;

        // Assert
        Assert.Equal("en", item.m_localization.m_language);
        Assert.Equal("en-US", item.m_localization.m_culture);
    }
}

public class WindowConfigItemTests
{
    [Fact]
    public void WindowConfigItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new WindowConfigItem();

        // Assert
        Assert.Equal(100, item.m_left);
        Assert.Equal(100, item.m_top);
        Assert.Equal(1224, item.m_width);
        Assert.Equal(800, item.m_height);
        Assert.False(item.m_is_maximized);
    }

    [Fact]
    public void WindowConfigItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new WindowConfigItem();

        // Act
        item.m_left = 200;
        item.m_top = 150;
        item.m_width = 1920;
        item.m_height = 1080;
        item.m_is_maximized = true;

        // Assert
        Assert.Equal(200, item.m_left);
        Assert.Equal(150, item.m_top);
        Assert.Equal(1920, item.m_width);
        Assert.Equal(1080, item.m_height);
        Assert.True(item.m_is_maximized);
    }

    [Fact]
    public void WindowConfigItem_SetNegativePosition_ShouldAllowNegativeValues()
    {
        // Arrange
        var item = new WindowConfigItem();

        // Act
        item.m_left = -100;
        item.m_top = -50;

        // Assert
        Assert.Equal(-100, item.m_left);
        Assert.Equal(-50, item.m_top);
    }

    [Fact]
    public void WindowConfigItem_SetZeroDimensions_ShouldAllowZeroValues()
    {
        // Arrange
        var item = new WindowConfigItem();

        // Act
        item.m_width = 0;
        item.m_height = 0;

        // Assert
        Assert.Equal(0, item.m_width);
        Assert.Equal(0, item.m_height);
    }
}

public class LocalizationConfigItemTests
{
    [Fact]
    public void LocalizationConfigItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new LocalizationConfigItem();

        // Assert
        Assert.Equal("fr", item.m_language);
        Assert.Equal("fr-CH", item.m_culture);
    }

    [Fact]
    public void LocalizationConfigItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new LocalizationConfigItem();

        // Act
        item.m_language = "en";
        item.m_culture = "en-US";

        // Assert
        Assert.Equal("en", item.m_language);
        Assert.Equal("en-US", item.m_culture);
    }

    [Fact]
    public void LocalizationConfigItem_SetEmptyLanguage_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new LocalizationConfigItem();

        // Act
        item.m_language = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.m_language);
    }

    [Fact]
    public void LocalizationConfigItem_SetEmptyCulture_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new LocalizationConfigItem();

        // Act
        item.m_culture = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.m_culture);
    }

    [Fact]
    public void LocalizationConfigItem_SetDifferentLanguages_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new LocalizationConfigItem();

        // Act & Assert - Test different languages
        item.m_language = "de";
        item.m_culture = "de-DE";
        Assert.Equal("de", item.m_language);
        Assert.Equal("de-DE", item.m_culture);

        item.m_language = "fr";
        item.m_culture = "fr-FR";
        Assert.Equal("fr", item.m_language);
        Assert.Equal("fr-FR", item.m_culture);
    }
}
