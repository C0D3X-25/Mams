using Mams_App.src.receipts;
using System.Collections.ObjectModel;

namespace Mams_Test.receipts;

public class ReceiptHandlerItemTests
{
    [Fact]
    public void ReceiptHandlerItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptHandlerItem();

        // Assert
        Assert.NotNull(item.receipt_item);
        Assert.NotNull(item.receipt_client_item);
        Assert.NotNull(item.receipt_supplier_item);
        Assert.NotNull(item.receipt_product_items);
        Assert.Empty(item.receipt_product_items);
    }

    [Fact]
    public void ReceiptHandlerItem_SetReceiptItem_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptHandlerItem();
        var receipt = new ReceiptItem { receipt_id = 1, receipt_number = "REC-001" };

        // Act
        item.receipt_item = receipt;

        // Assert
        Assert.Equal(1, item.receipt_item.receipt_id);
        Assert.Equal("REC-001", item.receipt_item.receipt_number);
    }

    [Fact]
    public void ReceiptHandlerItem_SetReceiptClientItem_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptHandlerItem();
        var client = new ReceiptClientItem { fk_client_id = 1, fk_receipt_id = 2 };

        // Act
        item.receipt_client_item = client;

        // Assert
        Assert.Equal(1, item.receipt_client_item.fk_client_id);
        Assert.Equal(2, item.receipt_client_item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptHandlerItem_SetReceiptSupplierItem_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptHandlerItem();
        var supplier = new ReceiptSupplierItem { fk_supplier_id = 1, fk_receipt_id = 2 };

        // Act
        item.receipt_supplier_item = supplier;

        // Assert
        Assert.Equal(1, item.receipt_supplier_item.fk_supplier_id);
        Assert.Equal(2, item.receipt_supplier_item.fk_receipt_id);
    }

    [Fact]
    public void ReceiptHandlerItem_SetReceiptProductItems_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptHandlerItem();
        var products = new ObservableCollection<ReceiptProductItem>
        {
            new ReceiptProductItem { receipt_product_id = 1 },
            new ReceiptProductItem { receipt_product_id = 2 }
        };

        // Act
        item.receipt_product_items = products;

        // Assert
        Assert.Equal(2, item.receipt_product_items.Count);
        Assert.Equal(1, item.receipt_product_items[0].receipt_product_id);
        Assert.Equal(2, item.receipt_product_items[1].receipt_product_id);
    }

    [Fact]
    public void ReceiptHandlerItem_AddReceiptProductItem_ShouldAddToCollection()
    {
        // Arrange
        var item = new ReceiptHandlerItem();
        var product = new ReceiptProductItem { receipt_product_id = 1 };

        // Act
        item.receipt_product_items.Add(product);

        // Assert
        Assert.Single(item.receipt_product_items);
        Assert.Equal(1, item.receipt_product_items[0].receipt_product_id);
    }
}
