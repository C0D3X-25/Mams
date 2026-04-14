using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.productsCategories;

namespace Mams_Test.integration.productsCategories;

/// <summary>
/// Integration tests for <see cref="ProductCategoryModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ProductCategoryModelIntegrationTests : IntegrationTestBase
{
    private readonly ProductCategoryModel _model = new();

    public ProductCategoryModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new ProductCategoryItem { product_category_name = "Miel" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new ProductCategoryItem { product_category_name = "Miel" };
        _model.saveItem(item);

        var duplicate = new ProductCategoryItem { product_category_name = "Miel" };
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
        var item = new ProductCategoryItem { product_category_name = "Miel" };
        var insertResponse = _model.saveItem(item);

        var updated = new ProductCategoryItem
        {
            product_category_id = insertResponse.returned_id,
            product_category_name = "Cire"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        // Verify the name was changed
        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Cire", fetched.returned_item!.product_category_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new ProductCategoryItem { product_category_name = "Propolis" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        // Row should be persisted after outer commit
        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Propolis", fetched.returned_item!.product_category_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new ProductCategoryItem { product_category_name = "Propolis" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.rollbackTransaction();

        // Row should NOT exist after rollback
        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        var item = new ProductCategoryItem { product_category_name = "Gelée royale" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Gelée royale", response.returned_item.product_category_name);
        Assert.Equal(saved.returned_id, response.returned_item.product_category_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getCategoryIdByName
    // ─────────────────────────────────────────────

    [Fact]
    public void GetCategoryIdByName_ExistingName_ReturnsId()
    {
        var item = new ProductCategoryItem { product_category_name = "Traitement" };
        var saved = _model.saveItem(item);

        int id = _model.getCategoryIdByName("Traitement");

        Assert.Equal(saved.returned_id, id);
    }

    [Fact]
    public void GetCategoryIdByName_NonExistingName_ReturnsZero()
    {
        int id = _model.getCategoryIdByName("NonExistant");

        Assert.Equal(0, id);
    }

    /// <summary>
    /// This is the exact scenario that caused the transaction bug:
    /// calling <see cref="ProductCategoryModel.getCategoryIdByName"/> while
    /// an outer transaction is active.
    /// </summary>
    [Fact]
    public void GetCategoryIdByName_InsideActiveTransaction_DoesNotThrow()
    {
        // Pre-insert a category outside any transaction
        var item = new ProductCategoryItem { product_category_name = "Traitement" };
        var saved = _model.saveItem(item);

        // Now start an outer transaction (simulating ReceiptHandlerModel.saveItem)
        ABaseModel.startTransaction();

        // This call previously threw because the MySqlCommand was not
        // associated with the active transaction
        int id = _model.getCategoryIdByName("Traitement");

        ABaseModel.commitTransaction();

        Assert.Equal(saved.returned_id, id);
    }

    [Fact]
    public void GetCategoryIdByName_InsideNestedTransaction_DoesNotThrow()
    {
        var item = new ProductCategoryItem { product_category_name = "Traitement" };
        var saved = _model.saveItem(item);

        // Simulate double-nesting (outer handler → inner model)
        ABaseModel.startTransaction();
        ABaseModel.startTransaction();

        int id = _model.getCategoryIdByName("Traitement");

        ABaseModel.commitTransaction();
        ABaseModel.commitTransaction();

        Assert.Equal(saved.returned_id, id);
    }

    // ─────────────────────────────────────────────
    //  getActiveProductCategories
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveProductCategories_ReturnsOnlyNonArchived()
    {
        // Insert an active category
        var active = new ProductCategoryItem { product_category_name = "Miel" };
        _model.saveItem(active);

        // Insert another and archive it
        var archived = new ProductCategoryItem { product_category_name = "Cire" };
        var archivedSave = _model.saveItem(archived);
        _model.deleteItem(archivedSave.returned_id.ToString());

        var categories = _model.getActiveProductCategories();

        Assert.Single(categories);
        Assert.Equal("Miel", categories[0].product_category_name);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new ProductCategoryItem { product_category_name = "Miel" };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        // The item should still exist but be archived (not returned by getActive)
        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.product_category_archive);
    }
}
