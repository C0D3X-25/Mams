using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.productsShapes;
using MySqlConnector;

namespace Mams_Test.integration.productsShapes;

/// <summary>
/// Integration tests for <see cref="ProductShapeModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ProductShapeModelIntegrationTests : IntegrationTestBase
{
    private readonly ProductShapeModel _model = new();

    public ProductShapeModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        _model.saveItem(item);

        var duplicate = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var response = _model.saveItem(duplicate);

        Assert.False(response.is_success);
        Assert.Equal(EErrors.ALREADY_EXISTS, response.error);
    }

    // ─────────────────────────────────────────────
    //  saveItem – UPDATE
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Update_ModifiesExistingRow()
    {
        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var insertResponse = _model.saveItem(item);

        var updated = new ProductShapeItem
        {
            product_shape_id = insertResponse.returned_id,
            product_shape_name = "Pot 250g"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Pot 250g", fetched.returned_item!.product_shape_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Pot 500g", fetched.returned_item!.product_shape_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.rollbackTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Pot 500g", response.returned_item.product_shape_name);
        Assert.Equal(saved.returned_id, response.returned_item.product_shape_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveProductShapes
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveProductShapes_ReturnsOnlyNonArchived()
    {
        var active = new ProductShapeItem { product_shape_name = "Pot 500g" };
        _model.saveItem(active);

        var archived = new ProductShapeItem { product_shape_name = "Pot 250g" };
        var archivedSave = _model.saveItem(archived);
        _model.deleteItem(archivedSave.returned_id.ToString());

        var shapes = _model.getActiveProductShapes();

        Assert.Contains(shapes, s => s.product_shape_name == "Pot 500g");
        Assert.DoesNotContain(shapes, s => s.product_shape_name == "Pot 250g");
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new ProductShapeItem { product_shape_name = "Pot 500g" };
        var saved = _model.saveItem(item);

        // Create a product referencing this shape so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO products_types (product_type_name) VALUES ('TestType')";
            cmd.ExecuteNonQuery();
            long typeId = cmd.LastInsertedId;
            cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('TestCat')";
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO products (product_name, fk_product_type_id, fk_product_category_id, fk_product_shape_id) " +
                $"VALUES ('TestProduct', {typeId}, LAST_INSERT_ID(), {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.product_shape_archive);
    }
}
