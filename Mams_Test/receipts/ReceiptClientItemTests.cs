using Mams_App.src.receipts;

namespace Mams_Test.receipts;

public class ReceiptClientItemTests
{
    [Fact]
    public void ReceiptClientItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptClientItem();

        // Assert
        Assert.Equal(0, item.fk_client_id);
        Assert.Equal(0, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptClientItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ReceiptClientItem();

        // Act
        item.fk_client_id = 1;
        item.fk_receipt_id = 2;

        // Assert
        Assert.Equal(1, item.fk_client_id);
        Assert.Equal(2, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptClientItem_SetNegativeForeignKeys_ShouldAllowNegativeValues()
    {
        // Arrange
        var item = new ReceiptClientItem();

        // Act
        item.fk_client_id = -1;
        item.fk_receipt_id = -2;

        // Assert
        Assert.Equal(-1, item.fk_client_id);
        Assert.Equal(-2, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptClientItem_SetZeroForeignKeys_ShouldAllowZeroValues()
    {
        // Arrange
        var item = new ReceiptClientItem { fk_client_id = 5, fk_receipt_id = 10 };

        // Act
        item.fk_client_id = 0;
        item.fk_receipt_id = 0;

        // Assert
        Assert.Equal(0, item.fk_client_id);
        Assert.Equal(0, item.fk_receipt_id);
    }
}
