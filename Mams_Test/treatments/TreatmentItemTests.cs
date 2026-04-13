using Mams_App.src.treatments;

namespace Mams_Test.treatments;

public class TreatmentItemTests
{
    [Fact]
    public void TreatmentItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new TreatmentItem();

        // Assert
        Assert.Equal(0, item.treatment_id);
        Assert.Equal(string.Empty, item.treatment_date);
        Assert.Equal(0, item.treatment_hive_count);
        Assert.Equal(0m, item.treatment_dose_per_hive);
        Assert.Equal(0, item.fk_beehive_id);
        Assert.Equal(0, item.fk_product_id);
        Assert.Equal(0, item.fk_dose_unit_id);
        Assert.Equal(string.Empty, item.beehive_name);
        Assert.Equal(string.Empty, item.beehive_number);
        Assert.Equal(string.Empty, item.product_name);
        Assert.Equal(string.Empty, item.region_name);
        Assert.Equal(string.Empty, item.dose_unit_name);
        Assert.Equal(0m, item.treatment_dose_total);
    }

    [Fact]
    public void TreatmentItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new TreatmentItem();

        // Act
        item.treatment_id = 1;
        item.treatment_date = "15.06.2025";
        item.treatment_hive_count = 10;
        item.treatment_dose_per_hive = 5.5m;
        item.fk_beehive_id = 3;
        item.fk_product_id = 7;
        item.fk_dose_unit_id = 2;
        item.beehive_name = "Rucher des Alpes";
        item.beehive_number = "CH-001";
        item.product_name = "Acide oxalique";
        item.region_name = "Fribourg";
        item.dose_unit_name = "ml";

        // Assert
        Assert.Equal(1, item.treatment_id);
        Assert.Equal("15.06.2025", item.treatment_date);
        Assert.Equal(10, item.treatment_hive_count);
        Assert.Equal(5.5m, item.treatment_dose_per_hive);
        Assert.Equal(3, item.fk_beehive_id);
        Assert.Equal(7, item.fk_product_id);
        Assert.Equal(2, item.fk_dose_unit_id);
        Assert.Equal("Rucher des Alpes", item.beehive_name);
        Assert.Equal("CH-001", item.beehive_number);
        Assert.Equal("Acide oxalique", item.product_name);
        Assert.Equal("Fribourg", item.region_name);
        Assert.Equal("ml", item.dose_unit_name);
    }

    [Fact]
    public void TreatmentItem_DoseTotal_ShouldBeCalculated()
    {
        // Arrange
        var item = new TreatmentItem
        {
            treatment_hive_count = 10,
            treatment_dose_per_hive = 5.5m
        };

        // Act & Assert
        Assert.Equal(55.0m, item.treatment_dose_total);
    }

    [Fact]
    public void TreatmentItem_DoseTotal_WithZeroHives_ShouldBeZero()
    {
        // Arrange
        var item = new TreatmentItem
        {
            treatment_hive_count = 0,
            treatment_dose_per_hive = 5.5m
        };

        // Act & Assert
        Assert.Equal(0m, item.treatment_dose_total);
    }

    [Fact]
    public void TreatmentItem_DoseTotal_WithZeroDose_ShouldBeZero()
    {
        // Arrange
        var item = new TreatmentItem
        {
            treatment_hive_count = 10,
            treatment_dose_per_hive = 0m
        };

        // Act & Assert
        Assert.Equal(0m, item.treatment_dose_total);
    }
}
