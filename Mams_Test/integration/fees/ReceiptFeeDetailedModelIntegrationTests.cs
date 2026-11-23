using Mams_App.src.entities;
using Mams_App.src.fees;
using Mams_App.src.models;
using Mams_App.src.products;
using Mams_App.src.receipts;
using Mams_App.src.suppliers;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Mams_Test.integration.fees;

/// <summary>
/// Integration tests for <see cref="ReceiptFeeDetailedModel"/> that verify
/// composite save/delete operations across multiple tables.
/// </summary>
[Collection("Database")]
public class ReceiptFeeDetailedModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptFeeDetailedModel _model = new();

    public ReceiptFeeDetailedModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

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

    private static ReceiptFeeDetailedItem createFeeItem(int entityId, int productId)
    {
        return new ReceiptFeeDetailedItem
        {
            entity = new EntityItem { entity_id = entityId },
            supplier = new SupplierItem(),
            receipt_supplier = new ReceiptSupplierItem(),
            receipt = new ReceiptItem
            {
                receipt_date_created = DateTime.Now.ToString("dd.MM.yyyy")
            },
            receipt_products = new ObservableCollection<ReceiptProductItem>
            {
                new()
                {
                    receipt_product_quantity = 10,
                    receipt_product_unity_price = 25.50m,
                    product_item = new ProductItem { product_id = productId },
                    product_lot_item = new Mams_App.src.productsLots.ProductLotItem()
                }
            }
        };
    }

    // ─────────────────────────────────────────────
    //  saveItem
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_CreatesReceiptWithSupplierAndProducts()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_CreatesSupplierAutomatically()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);
        _model.saveItem(item);

        // Verify supplier was auto-created for the entity
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM suppliers WHERE fk_entity_id = {entityId}";
        var count = Convert.ToInt32(cmd.ExecuteScalar());

        Assert.True(count > 0);
    }

    [Fact]
    public void SaveItem_CalculatesTotalPrice()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);
        var response = _model.saveItem(item);

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(255.00m, fetched.returned_item!.receipt.receipt_total_price);
    }

    [Fact]
    public void SaveItem_MultipleProducts_AllSaved()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId1 = insertProduct("Cadre Dadant");
        int productId2 = insertProduct("Cire gaufrée");

        var item = new ReceiptFeeDetailedItem
        {
            entity = new EntityItem { entity_id = entityId },
            supplier = new SupplierItem(),
            receipt_supplier = new ReceiptSupplierItem(),
            receipt = new ReceiptItem
            {
                receipt_date_created = DateTime.Now.ToString("dd.MM.yyyy")
            },
            receipt_products = new ObservableCollection<ReceiptProductItem>
            {
                new()
                {
                    receipt_product_quantity = 10,
                    receipt_product_unity_price = 25.00m,
                    product_item = new ProductItem { product_id = productId1 },
                    product_lot_item = new Mams_App.src.productsLots.ProductLotItem()
                },
                new()
                {
                    receipt_product_quantity = 5,
                    receipt_product_unity_price = 15.00m,
                    product_item = new ProductItem { product_id = productId2 },
                    product_lot_item = new Mams_App.src.productsLots.ProductLotItem()
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
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Fournisseur Alpha", response.returned_item.entity.entity_name);
        Assert.Single(response.returned_item.receipt_products);
        Assert.Equal("Cadre Dadant", response.returned_item.receipt_products[0].product_item.product_name);
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
    public void GetAllItems_ReturnsFeeReceiptsOnly()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.Contains(response.returned_items!, f => f.entity.entity_name == "Fournisseur Alpha");
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_RemovesReceiptAndAssociatedData()
    {
        int entityId = insertEntity("Fournisseur Alpha");
        int productId = insertProduct("Cadre Dadant");

        var item = createFeeItem(entityId, productId);
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }
}
