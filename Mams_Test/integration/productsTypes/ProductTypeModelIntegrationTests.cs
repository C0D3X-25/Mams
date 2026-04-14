using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.productsTypes;
using MySqlConnector;

namespace Mams_Test.integration.productsTypes;

/// <summary>
/// Integration tests for <see cref="ProductTypeModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ProductTypeModelIntegrationTests : IntegrationTestBase
{
    private readonly ProductTypeModel _model = new();

    public ProductTypeModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new ProductTypeItem { product_type_name = "Alimentaire" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
        _model.saveItem(item);

        var duplicate = new ProductTypeItem { product_type_name = "Alimentaire" };
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
        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
        var insertResponse = _model.saveItem(item);

        var updated = new ProductTypeItem
        {
            product_type_id = insertResponse.returned_id,
            product_type_name = "Cosmétique"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Cosmétique", fetched.returned_item!.product_type_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Alimentaire", fetched.returned_item!.product_type_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
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
        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Alimentaire", response.returned_item.product_type_name);
        Assert.Equal(saved.returned_id, response.returned_item.product_type_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveProductTypes
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveProductTypes_ReturnsOnlyNonArchived()
    {
        var active = new ProductTypeItem { product_type_name = "Alimentaire" };
        _model.saveItem(active);

        var archived = new ProductTypeItem { product_type_name = "Cosmétique" };
        var archivedSave = _model.saveItem(archived);
        _model.deleteItem(archivedSave.returned_id.ToString());

        var types = _model.getActiveProductTypes();

        Assert.Single(types);
        Assert.Equal("Alimentaire", types[0].product_type_name);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new ProductTypeItem { product_type_name = "Alimentaire" };
        var saved = _model.saveItem(item);

        // Create a product referencing this type so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('TestCat')";
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO products (product_name, fk_product_type_id, fk_product_category_id) " +
                $"VALUES ('TestProduct', {saved.returned_id}, LAST_INSERT_ID())";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.product_type_archive);
    }
}
