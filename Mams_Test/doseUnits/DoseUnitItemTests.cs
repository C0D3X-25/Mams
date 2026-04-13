using Mams_App.src.doseUnits;

namespace Mams_Test.doseUnits;

public class DoseUnitItemTests
{
    [Fact]
    public void DoseUnitItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new DoseUnitItem();

        // Assert
        Assert.Equal(0, item.dose_unit_id);
        Assert.Equal(string.Empty, item.dose_unit_name);
        Assert.Equal(string.Empty, item.dose_unit_archive);
    }

    [Fact]
    public void DoseUnitItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new DoseUnitItem();

        // Act
        item.dose_unit_id = 1;
        item.dose_unit_name = "ml";
        item.dose_unit_archive = "2025-01-15";

        // Assert
        Assert.Equal(1, item.dose_unit_id);
        Assert.Equal("ml", item.dose_unit_name);
        Assert.Equal("2025-01-15", item.dose_unit_archive);
    }

    [Fact]
    public void DoseUnitItem_SetName_Empty_ShouldBeEmpty()
    {
        // Arrange
        var item = new DoseUnitItem { dose_unit_name = "ml" };

        // Act
        item.dose_unit_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.dose_unit_name);
    }

    [Fact]
    public void DoseUnitItem_MultipleInstances_ShouldBeIndependent()
    {
        // Arrange
        var item1 = new DoseUnitItem { dose_unit_id = 1, dose_unit_name = "ml" };
        var item2 = new DoseUnitItem { dose_unit_id = 2, dose_unit_name = "l" };

        // Assert
        Assert.NotEqual(item1.dose_unit_id, item2.dose_unit_id);
        Assert.NotEqual(item1.dose_unit_name, item2.dose_unit_name);
    }
}
