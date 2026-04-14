using Mams_App.src.treatmentStocks;

namespace Mams_Test.treatmentStocks;

public class TreatmentStockItemTests
{
    [Fact]
    public void TreatmentStockItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new TreatmentStockItem();

        // Assert
        Assert.Equal(0, item.treatment_stock_id);
        Assert.Equal(string.Empty, item.treatment_stock_purchase_date);
        Assert.Equal(0m, item.treatment_stock_initial_quantity);
        Assert.Equal(string.Empty, item.treatment_stock_first_used_date);
        Assert.Equal(string.Empty, item.treatment_stock_last_used_date);
        Assert.Equal(0, item.fk_product_id);
        Assert.Equal(0, item.fk_dose_unit_id);
        Assert.Equal(0, item.fk_supplier_id);
        Assert.Equal(string.Empty, item.product_name);
        Assert.Equal(string.Empty, item.dose_unit_name);
        Assert.Equal(string.Empty, item.supplier_name);
        Assert.Equal(0m, item.treatment_stock_used_quantity);
        Assert.Equal(0m, item.treatment_stock_remaining_quantity);
    }

    [Fact]
    public void TreatmentStockItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new TreatmentStockItem();

        // Act
        item.treatment_stock_id = 1;
        item.treatment_stock_purchase_date = "01.06.2025";
        item.treatment_stock_initial_quantity = 300m;
        item.treatment_stock_first_used_date = "15.06.2025";
        item.treatment_stock_last_used_date = "20.07.2025";
        item.fk_product_id = 5;
        item.fk_dose_unit_id = 2;
        item.fk_supplier_id = 3;
        item.product_name = "Acide formique";
        item.dose_unit_name = "ml";
        item.supplier_name = "ApiSuisse";
        item.treatment_stock_used_quantity = 55m;

        // Assert
        Assert.Equal(1, item.treatment_stock_id);
        Assert.Equal("01.06.2025", item.treatment_stock_purchase_date);
        Assert.Equal(300m, item.treatment_stock_initial_quantity);
        Assert.Equal("15.06.2025", item.treatment_stock_first_used_date);
        Assert.Equal("20.07.2025", item.treatment_stock_last_used_date);
        Assert.Equal(5, item.fk_product_id);
        Assert.Equal(2, item.fk_dose_unit_id);
        Assert.Equal(3, item.fk_supplier_id);
        Assert.Equal("Acide formique", item.product_name);
        Assert.Equal("ml", item.dose_unit_name);
        Assert.Equal("ApiSuisse", item.supplier_name);
        Assert.Equal(55m, item.treatment_stock_used_quantity);
    }

    [Fact]
    public void TreatmentStockItem_RemainingQuantity_ShouldBeCalculated()
    {
        // Arrange
        var item = new TreatmentStockItem
        {
            treatment_stock_initial_quantity = 300m,
            treatment_stock_used_quantity = 55m
        };

        // Act & Assert
        Assert.Equal(245m, item.treatment_stock_remaining_quantity);
    }

    [Fact]
    public void TreatmentStockItem_RemainingQuantity_WithNoUsage_ShouldEqualInitial()
    {
        // Arrange
        var item = new TreatmentStockItem
        {
            treatment_stock_initial_quantity = 300m,
            treatment_stock_used_quantity = 0m
        };

        // Act & Assert
        Assert.Equal(300m, item.treatment_stock_remaining_quantity);
    }

    [Fact]
    public void TreatmentStockItem_RemainingQuantity_FullyUsed_ShouldBeZero()
    {
        // Arrange
        var item = new TreatmentStockItem
        {
            treatment_stock_initial_quantity = 300m,
            treatment_stock_used_quantity = 300m
        };

        // Act & Assert
        Assert.Equal(0m, item.treatment_stock_remaining_quantity);
    }

    [Fact]
    public void TreatmentStockItem_DisplayName_ShouldFormatCorrectly()
    {
        // Arrange
        var item = new TreatmentStockItem
        {
            product_name = "Acide formique",
            supplier_name = "ApiSuisse",
            treatment_stock_initial_quantity = 300m,
            treatment_stock_used_quantity = 55m,
            dose_unit_name = "ml"
        };

        // Act & Assert
        Assert.Equal("Acide formique - ApiSuisse (245.00 ml)", item.display_name);
    }
}
