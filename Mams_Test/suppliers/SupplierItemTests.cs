using Mams_App.src.suppliers;

namespace MamsTest.suppliers;

public class SupplierItemTests
{
    [Fact]
    public void SupplierItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new SupplierItem();

        // Assert
        Assert.Equal(0, item.supplier_id);
        Assert.Equal(0, item.fk_entity_id);
    }

    [Fact]
    public void SupplierItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new SupplierItem();

        // Act
        item.supplier_id = 1;
        item.fk_entity_id = 5;

        // Assert
        Assert.Equal(1, item.supplier_id);
        Assert.Equal(5, item.fk_entity_id);
    }

    [Fact]
    public void SupplierItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new SupplierItem();

        // Act
        item.supplier_id = -1;

        // Assert
        Assert.Equal(-1, item.supplier_id);
    }

    [Fact]
    public void SupplierItem_SetForeignKeyEntity_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new SupplierItem { fk_entity_id = 5 };

        // Act
        item.fk_entity_id = 0;

        // Assert
        Assert.Equal(0, item.fk_entity_id);
    }

    [Fact]
    public void SupplierItem_SetNegativeForeignKey_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new SupplierItem();

        // Act
        item.fk_entity_id = -1;

        // Assert
        Assert.Equal(-1, item.fk_entity_id);
    }
}
