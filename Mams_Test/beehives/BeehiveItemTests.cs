using Mams_App.src.beehives;

namespace Mams_Test.beehives;

public class BeehiveItemTests
{
    [Fact]
    public void BeehiveItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new BeehiveItem();

        // Assert
        Assert.Equal(0, item.beehive_id);
        Assert.Equal(string.Empty, item.beehive_name);
        Assert.Equal(string.Empty, item.beehive_number);
        Assert.Equal(string.Empty, item.beehive_archive);
        Assert.Equal(0, item.fk_region_id);
        Assert.Equal(string.Empty, item.region_name);
    }

    [Fact]
    public void BeehiveItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new BeehiveItem();

        // Act
        item.beehive_id = 1;
        item.beehive_name = "Test Beehive";
        item.beehive_number = "CH-12345";
        item.beehive_archive = "2024-01-01";
        item.fk_region_id = 7;
        item.region_name = "Fribourg";

        // Assert
        Assert.Equal(1, item.beehive_id);
        Assert.Equal("Test Beehive", item.beehive_name);
        Assert.Equal("CH-12345", item.beehive_number);
        Assert.Equal("2024-01-01", item.beehive_archive);
        Assert.Equal(7, item.fk_region_id);
        Assert.Equal("Fribourg", item.region_name);
    }

    [Fact]
    public void BeehiveItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new BeehiveItem { beehive_name = "Initial Name" };

        // Act
        item.beehive_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.beehive_name);
    }

    [Fact]
    public void BeehiveItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new BeehiveItem();

        // Act
        item.beehive_id = -1;

        // Assert
        Assert.Equal(-1, item.beehive_id);
    }
}
