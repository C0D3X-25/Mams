using Mams_App.src.receipts;
using Mams_App.src.products;
using Mams_App.src.productsLots;

namespace Mams_Test.receipts;

public class ReceiptProductItemTests
{
    [Fact]
    public void ReceiptProductItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptProductItem();

        // Assert
        Assert.Equal(0, item.receipt_product_id);
        Assert.Equal(0, item.receipt_product_quantity);
        Assert.Equal(0.0M, item.receipt_product_unity_price);
        Assert.Equal(0, item.fk_receipt_id);
        Assert.NotNull(item.product_item);
        Assert.NotNull(item.product_lot_item);
    }

    [Fact]
    public void ReceiptProductItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ReceiptProductItem();

        // Act
        item.receipt_product_id = 1;
        item.receipt_product_quantity = 10;
        item.receipt_product_unity_price = 25.50M;
        item.fk_receipt_id = 5;

        // Assert
        Assert.Equal(1, item.receipt_product_id);
        Assert.Equal(10, item.receipt_product_quantity);
        Assert.Equal(25.50M, item.receipt_product_unity_price);
        Assert.Equal(5, item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptProductItem_SetProductItem_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProductItem();
        var product = new ProductItem { product_id = 1, product_name = "Test Product" };

        // Act
        item.product_item = product;

        // Assert
        Assert.Equal(1, item.product_item.product_id);
        Assert.Equal("Test Product", item.product_item.product_name);
    }

    [Fact]
    public void ReceiptProductItem_SetProductLotItem_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProductItem();
        var lot = new ProductLotItem { product_lot_id = 1, product_lot_name = "Test Lot" };

        // Act
        item.product_lot_item = lot;

        // Assert
        Assert.Equal(1, item.product_lot_item.product_lot_id);
        Assert.Equal("Test Lot", item.product_lot_item.product_lot_name);
    }

    [Fact]
    public void ReceiptProductItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ReceiptProductItem();

        // Act
        item.receipt_product_id = -1;

        // Assert
        Assert.Equal(-1, item.receipt_product_id);
    }

    [Fact]
    public void ReceiptProductItem_SetZeroQuantity_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ReceiptProductItem { receipt_product_quantity = 10 };

        // Act
        item.receipt_product_quantity = 0;

        // Assert
        Assert.Equal(0, item.receipt_product_quantity);
    }

    [Fact]
    public void ReceiptProductItem_SetNegativeUnityPrice_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ReceiptProductItem();

        // Act
        item.receipt_product_unity_price = -10.0M;

        // Assert
        Assert.Equal(-10.0M, item.receipt_product_unity_price);
    }
}
