using Mams_App.src.fees;
using Mams_App.src.entities;
using Mams_App.src.suppliers;
using Mams_App.src.receipts;
using System.Collections.ObjectModel;

namespace Mams_Test.fees;

public class ReceiptFeeDetailedItemTests
{
    [Fact]
    public void ReceiptFeeDetailedItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptFeeDetailedItem();

        // Assert
        Assert.NotNull(item.entity);
        Assert.NotNull(item.supplier);
        Assert.NotNull(item.receipt_supplier);
        Assert.NotNull(item.receipt);
        Assert.NotNull(item.receipt_products);
        Assert.Empty(item.receipt_products);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_SetEntity_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var entity = new EntityItem { entity_id = 1, entity_name = "Test Entity" };

        // Act
        item.entity = entity;

        // Assert
        Assert.Equal(1, item.entity.entity_id);
        Assert.Equal("Test Entity", item.entity.entity_name);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_SetSupplier_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var supplier = new SupplierItem { supplier_id = 1, fk_entity_id = 2 };

        // Act
        item.supplier = supplier;

        // Assert
        Assert.Equal(1, item.supplier.supplier_id);
        Assert.Equal(2, item.supplier.fk_entity_id);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_SetReceiptSupplier_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var receiptSupplier = new ReceiptSupplierItem { fk_supplier_id = 1, fk_receipt_id = 2 };

        // Act
        item.receipt_supplier = receiptSupplier;

        // Assert
        Assert.Equal(1, item.receipt_supplier.fk_supplier_id);
        Assert.Equal(2, item.receipt_supplier.fk_receipt_id);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_SetReceipt_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var receipt = new ReceiptItem { receipt_id = 1, receipt_number = "REC-001" };

        // Act
        item.receipt = receipt;

        // Assert
        Assert.Equal(1, item.receipt.receipt_id);
        Assert.Equal("REC-001", item.receipt.receipt_number);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_SetReceiptProducts_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var products = new ObservableCollection<ReceiptProductItem>
        {
            new ReceiptProductItem { receipt_product_id = 1 },
            new ReceiptProductItem { receipt_product_id = 2 }
        };

        // Act
        item.receipt_products = products;

        // Assert
        Assert.Equal(2, item.receipt_products.Count);
        Assert.Equal(1, item.receipt_products[0].receipt_product_id);
        Assert.Equal(2, item.receipt_products[1].receipt_product_id);
    }

    [Fact]
    public void ReceiptFeeDetailedItem_AddReceiptProduct_ShouldAddToCollection()
    {
        // Arrange
        var item = new ReceiptFeeDetailedItem();
        var product = new ReceiptProductItem { receipt_product_id = 1 };

        // Act
        item.receipt_products.Add(product);

        // Assert
        Assert.Single(item.receipt_products);
        Assert.Equal(1, item.receipt_products[0].receipt_product_id);
    }
}
