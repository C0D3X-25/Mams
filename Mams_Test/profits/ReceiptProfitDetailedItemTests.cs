using Mams_App.src.profits;
using Mams_App.src.clients;
using Mams_App.src.entities;
using Mams_App.src.receipts;
using System.Collections.ObjectModel;

namespace Mams_Test.profits;

public class ReceiptProfitDetailedItemTests
{
    [Fact]
    public void ReceiptProfitDetailedItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ReceiptProfitDetailedItem();

        // Assert
        Assert.NotNull(item.entity);
        Assert.NotNull(item.client);
        Assert.NotNull(item.receipt_client);
        Assert.NotNull(item.receipt);
        Assert.NotNull(item.receipt_products);
        Assert.Empty(item.receipt_products);
    }

    [Fact]
    public void ReceiptProfitDetailedItem_SetEntity_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
        var entity = new EntityItem { entity_id = 1, entity_name = "Test Entity" };

        // Act
        item.entity = entity;

        // Assert
        Assert.Equal(1, item.entity.entity_id);
        Assert.Equal("Test Entity", item.entity.entity_name);
    }

    [Fact]
    public void ReceiptProfitDetailedItem_SetClient_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
        var client = new ClientItem { client_id = 1, fk_entity_id = 2 };

        // Act
        item.client = client;

        // Assert
        Assert.Equal(1, item.client.client_id);
        Assert.Equal(2, item.client.fk_entity_id);
    }

    [Fact]
    public void ReceiptProfitDetailedItem_SetReceiptClient_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
        var receiptClient = new ReceiptClientItem { fk_client_id = 1, fk_receipt_id = 2 };

        // Act
        item.receipt_client = receiptClient;

        // Assert
        Assert.Equal(1, item.receipt_client.fk_client_id);
        Assert.Equal(2, item.receipt_client.fk_receipt_id);
    }

    [Fact]
    public void ReceiptProfitDetailedItem_SetReceipt_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
        var receipt = new ReceiptItem { receipt_id = 1, receipt_number = "REC-001" };

        // Act
        item.receipt = receipt;

        // Assert
        Assert.Equal(1, item.receipt.receipt_id);
        Assert.Equal("REC-001", item.receipt.receipt_number);
    }

    [Fact]
    public void ReceiptProfitDetailedItem_SetReceiptProducts_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
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
    public void ReceiptProfitDetailedItem_AddReceiptProduct_ShouldAddToCollection()
    {
        // Arrange
        var item = new ReceiptProfitDetailedItem();
        var product = new ReceiptProductItem { receipt_product_id = 1 };

        // Act
        item.receipt_products.Add(product);

        // Assert
        Assert.Single(item.receipt_products);
        Assert.Equal(1, item.receipt_products[0].receipt_product_id);
    }
}
