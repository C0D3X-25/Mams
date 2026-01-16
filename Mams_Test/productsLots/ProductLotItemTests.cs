using Mams_App.src.productsLots;

namespace Mams_Test.productsLots;

public class ProductLotItemTests
{
    [Fact]
    public void ProductLotItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ProductLotItem();

        // Assert
        Assert.Equal(0, item.product_lot_id);
        Assert.Equal(string.Empty, item.product_lot_name);
        Assert.Equal(DateTime.Now.Year, item.product_lot_year);
        Assert.Equal(0, item.fk_beehive_id);
        Assert.Equal(string.Empty, item.beehive_name);
        Assert.Equal(string.Empty, item.product_lot_archive);
    }

    [Fact]
    public void ProductLotItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ProductLotItem();

        // Act
        item.product_lot_id = 1;
        item.product_lot_name = "Test Lot";
        item.product_lot_year = 2023;
        item.fk_beehive_id = 5;
        item.beehive_name = "Test Beehive";
        item.product_lot_archive = "2024-01-01";

        // Assert
        Assert.Equal(1, item.product_lot_id);
        Assert.Equal("Test Lot", item.product_lot_name);
        Assert.Equal(2023, item.product_lot_year);
        Assert.Equal(5, item.fk_beehive_id);
        Assert.Equal("Test Beehive", item.beehive_name);
        Assert.Equal("2024-01-01", item.product_lot_archive);
    }

    [Fact]
    public void ProductLotItem_SetEmptyName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new ProductLotItem { product_lot_name = "Initial Name" };

        // Act
        item.product_lot_name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.product_lot_name);
    }

    [Fact]
    public void ProductLotItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ProductLotItem();

        // Act
        item.product_lot_id = -1;

        // Assert
        Assert.Equal(-1, item.product_lot_id);
    }

    [Fact]
    public void ProductLotItem_SetZeroYear_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ProductLotItem();

        // Act
        item.product_lot_year = 0;

        // Assert
        Assert.Equal(0, item.product_lot_year);
    }

    [Fact]
    public void ProductLotItem_SetForeignKeyBeehive_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ProductLotItem { fk_beehive_id = 5 };

        // Act
        item.fk_beehive_id = 0;

        // Assert
        Assert.Equal(0, item.fk_beehive_id);
    }
}
