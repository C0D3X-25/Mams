using Mams_App.src.receipts;
using Mams_App.src.globals;

namespace Mams_Test.receipts;

public class ReceiptItemTests
{
    [Fact]
    public void ReceiptItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptItem();

        // Assert
        Assert.Equal(0, item.receipt_id);
        Assert.Equal(string.Empty, item.receipt_number);
        Assert.Equal(0.0M, item.receipt_total_price);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today).ToString(SGlobals.g_EU_DATE_FORMAT), item.receipt_date_created);
    }

    [Fact]
    public void ReceiptItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ReceiptItem();

        // Act
        item.receipt_id = 1;
        item.receipt_number = "REC-001";
        item.receipt_total_price = 150.50M;
        item.receipt_date_created = "01.01.2024";

        // Assert
        Assert.Equal(1, item.receipt_id);
        Assert.Equal("REC-001", item.receipt_number);
        Assert.Equal(150.50M, item.receipt_total_price);
        Assert.Equal("01.01.2024", item.receipt_date_created);
    }

    [Fact]
    public void ReceiptItem_SetEmptyNumber_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new ReceiptItem { receipt_number = "REC-001" };

        // Act
        item.receipt_number = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.receipt_number);
    }

    [Fact]
    public void ReceiptItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ReceiptItem();

        // Act
        item.receipt_id = -1;

        // Assert
        Assert.Equal(-1, item.receipt_id);
    }

    [Fact]
    public void ReceiptItem_SetZeroTotalPrice_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ReceiptItem { receipt_total_price = 100.0M };

        // Act
        item.receipt_total_price = 0.0M;

        // Assert
        Assert.Equal(0.0M, item.receipt_total_price);
    }

    [Fact]
    public void ReceiptItem_SetNegativeTotalPrice_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ReceiptItem();

        // Act
        item.receipt_total_price = -50.0M;

        // Assert
        Assert.Equal(-50.0M, item.receipt_total_price);
    }
}
