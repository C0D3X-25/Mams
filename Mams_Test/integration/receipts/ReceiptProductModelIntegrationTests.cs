using Mams_App.src.models;
using Mams_App.src.products;
using Mams_App.src.productsLots;
using Mams_App.src.receipts;
using MySqlConnector;

namespace Mams_Test.integration.receipts;

/// <summary>
/// Integration tests for <see cref="ReceiptProductModel"/> that verify
/// database operations for receipt product lines.
/// </summary>
[Collection("Database")]
public class ReceiptProductModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptProductModel _model = new();

    public ReceiptProductModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  Helper methods
    // ─────────────────────────────────────────────

    private static int insertReceipt()
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO receipts (receipt_number, receipt_total_price, receipt_date_created) " +
            "VALUES ('F-TEST', 0, '2025-01-15'); SELECT LAST_INSERT_ID();";
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

    // ─────────────────────────────────────────────
    //  saveItem
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        int receiptId = insertReceipt();
        int productId = insertProduct("Cadre Dadant");

        var item = new ReceiptProductItem
        {
            receipt_product_quantity = 10,
            receipt_product_unity_price = 5.50m,
            fk_receipt_id = receiptId,
            product_item = new ProductItem { product_id = productId },
            product_lot_item = new ProductLotItem()
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        int receiptId = insertReceipt();
        int productId = insertProduct("Cadre Dadant");

        var item = new ReceiptProductItem
        {
            receipt_product_quantity = 10,
            receipt_product_unity_price = 5.50m,
            fk_receipt_id = receiptId,
            product_item = new ProductItem { product_id = productId },
            product_lot_item = new ProductLotItem()
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(10, response.returned_item.receipt_product_quantity);
        Assert.Equal(5.50m, response.returned_item.receipt_product_unity_price);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getListItemWithReceiptID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetListItemWithReceiptID_ReturnsProductsForReceipt()
    {
        int receiptId = insertReceipt();
        int productId = insertProduct("Cadre Dadant");

        var item = new ReceiptProductItem
        {
            receipt_product_quantity = 10,
            receipt_product_unity_price = 5.50m,
            fk_receipt_id = receiptId,
            product_item = new ProductItem { product_id = productId },
            product_lot_item = new ProductLotItem()
        };
        _model.saveItem(item);

        var items = _model.getListItemWithReceiptID(receiptId.ToString());

        Assert.NotEmpty(items);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_RemovesProductLines()
    {
        int receiptId = insertReceipt();
        int productId = insertProduct("Cadre Dadant");

        var item = new ReceiptProductItem
        {
            receipt_product_quantity = 10,
            receipt_product_unity_price = 5.50m,
            fk_receipt_id = receiptId,
            product_item = new ProductItem { product_id = productId },
            product_lot_item = new ProductLotItem()
        };
        _model.saveItem(item);

        var response = _model.deleteItem(receiptId.ToString());

        Assert.True(response.is_success);

        var items = _model.getListItemWithReceiptID(receiptId.ToString());
        Assert.Empty(items);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsInsertedProducts()
    {
        int receiptId = insertReceipt();
        int productId = insertProduct("Cadre Dadant");

        var item = new ReceiptProductItem
        {
            receipt_product_quantity = 10,
            receipt_product_unity_price = 5.50m,
            fk_receipt_id = receiptId,
            product_item = new ProductItem { product_id = productId },
            product_lot_item = new ProductLotItem()
        };
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.NotEmpty(response.returned_items!);
    }
}
