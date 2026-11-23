using Mams_App.src.clients;
using Mams_App.src.entities;
using Mams_App.src.models;
using Mams_App.src.products;
using Mams_App.src.productsLots;
using Mams_App.src.profits;
using Mams_App.src.receipts;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Mams_Test.integration.profits;

/// <summary>
/// Integration tests for <see cref="ReceiptProfitDetailedModel"/> that verify
/// composite save/delete operations across multiple tables.
/// </summary>
[Collection("Database")]
public class ReceiptProfitDetailedModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptProfitDetailedModel _model = new();

    public ReceiptProfitDetailedModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  Helper methods
    // ─────────────────────────────────────────────

    private static int insertEntity(string name)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"INSERT INTO entities (entity_name) VALUES ('{name}'); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static int insertProduct(string name)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO products_types (product_type_name) VALUES ('Type Test'); SELECT LAST_INSERT_ID();";
        int typeId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText =
            "INSERT INTO products_categories (product_category_name) VALUES ('Cat Test'); SELECT LAST_INSERT_ID();";
        int catId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText =
            $"INSERT INTO products (product_name, product_weight, fk_product_type_id, fk_product_category_id, fk_product_shape_id) " +
            $"VALUES ('{name}', 500, {typeId}, {catId}, 1); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static ReceiptProfitDetailedItem createProfitItem(int entityId, int productId)
    {
        return new ReceiptProfitDetailedItem
        {
            entity = new EntityItem { entity_id = entityId },
            client = new ClientItem(),
            receipt_client = new ReceiptClientItem(),
            receipt = new ReceiptItem
            {
                receipt_date_created = DateTime.Now.ToString("dd.MM.yyyy")
            },
            receipt_products = new ObservableCollection<ReceiptProductItem>
            {
                new()
                {
                    receipt_product_quantity = 5,
                    receipt_product_unity_price = 12.00m,
                    product_item = new ProductItem { product_id = productId },
                    product_lot_item = new ProductLotItem()
                }
            }
        };
    }

    // ─────────────────────────────────────────────
    //  saveItem
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_CreatesReceiptWithClientAndProducts()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_CreatesClientAutomatically()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);
        _model.saveItem(item);

        // Verify client was auto-created for the entity
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM clients WHERE fk_entity_id = {entityId}";
        var count = Convert.ToInt32(cmd.ExecuteScalar());

        Assert.True(count > 0);
    }

    [Fact]
    public void SaveItem_CalculatesTotalPrice()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);
        var response = _model.saveItem(item);

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(60.00m, fetched.returned_item!.receipt.receipt_total_price);
    }

    [Fact]
    public void SaveItem_MultipleProducts_AllSaved()
    {
        int entityId = insertEntity("Client Alpha");
        int productId1 = insertProduct("Miel de Fleurs");
        int productId2 = insertProduct("Miel de Sapin");

        var item = new ReceiptProfitDetailedItem
        {
            entity = new EntityItem { entity_id = entityId },
            client = new ClientItem(),
            receipt_client = new ReceiptClientItem(),
            receipt = new ReceiptItem
            {
                receipt_date_created = DateTime.Now.ToString("dd.MM.yyyy")
            },
            receipt_products = new ObservableCollection<ReceiptProductItem>
            {
                new()
                {
                    receipt_product_quantity = 5,
                    receipt_product_unity_price = 12.00m,
                    product_item = new ProductItem { product_id = productId1 },
                    product_lot_item = new ProductLotItem()
                },
                new()
                {
                    receipt_product_quantity = 3,
                    receipt_product_unity_price = 18.00m,
                    product_item = new ProductItem { product_id = productId2 },
                    product_lot_item = new ProductLotItem()
                }
            }
        };

        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(2, fetched.returned_item!.receipt_products.Count);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ReturnsFullDetailedItem()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Client Alpha", response.returned_item.entity.entity_name);
        Assert.Single(response.returned_item.receipt_products);
        Assert.Equal("Miel de Fleurs", response.returned_item.receipt_products[0].product_item.product_name);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsProfitReceiptsOnly()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.Contains(response.returned_items!, p => p.entity.entity_name == "Client Alpha");
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_RemovesReceiptAndAssociatedData()
    {
        int entityId = insertEntity("Client Alpha");
        int productId = insertProduct("Miel de Fleurs");

        var item = createProfitItem(entityId, productId);
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }
}
