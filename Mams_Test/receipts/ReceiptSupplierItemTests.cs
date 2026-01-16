using Mams_App.src.receipts;

namespace Mams_Test.receipts;

public class ReceiptSupplierItemTests
{
    [Fact]
    public void ReceiptSupplierItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptSupplierItem();

        // Assert
        Assert.Equal(0, item.fk_supplier_id);
        Assert.Equal(0, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptSupplierItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ReceiptSupplierItem();

        // Act
        item.fk_supplier_id = 1;
        item.fk_receipt_id = 2;

        // Assert
        Assert.Equal(1, item.fk_supplier_id);
        Assert.Equal(2, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptSupplierItem_SetNegativeForeignKeys_ShouldAllowNegativeValues()
    {
        // Arrange
        var item = new ReceiptSupplierItem();

        // Act
        item.fk_supplier_id = -1;
        item.fk_receipt_id = -2;

        // Assert
        Assert.Equal(-1, item.fk_supplier_id);
        Assert.Equal(-2, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptSupplierItem_SetZeroForeignKeys_ShouldAllowZeroValues()
    {
        // Arrange
        var item = new ReceiptSupplierItem { fk_supplier_id = 5, fk_receipt_id = 10 };

        // Act
        item.fk_supplier_id = 0;
        item.fk_receipt_id = 0;

        // Assert
        Assert.Equal(0, item.fk_supplier_id);
        Assert.Equal(0, item.fk_receipt_id);
    }
}
