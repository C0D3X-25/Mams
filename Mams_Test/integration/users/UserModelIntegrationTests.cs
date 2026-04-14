using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.users;

namespace Mams_Test.integration.users;

/// <summary>
/// Integration tests for <see cref="UserModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class UserModelIntegrationTests : IntegrationTestBase
{
    private readonly UserModel _model = new();

    public UserModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveUser – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveUser_Insert_ReturnsId1()
    {
        var item = new UserItem
        {
            user_name = "Jean Apiculteur",
            user_phone = "0123456789",
            user_email = "jean@miel.com",
            user_city = "Dijon",
            user_address = "3 rue des Abeilles"
        };

        var response = _model.saveUser(item);

        Assert.True(response.is_success);
        Assert.Equal(1, response.returned_id);
    }

    // ─────────────────────────────────────────────
    //  saveUser – UPDATE
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveUser_Update_ModifiesExistingRow()
    {
        var item = new UserItem
        {
            user_name = "Jean Apiculteur",
            user_phone = "0123456789",
            user_email = "jean@miel.com",
            user_city = "Dijon",
            user_address = "3 rue des Abeilles"
        };
        _model.saveUser(item);

        var updated = new UserItem
        {
            user_name = "Pierre Apiculteur",
            user_phone = "0987654321",
            user_email = "pierre@miel.com",
            user_city = "Lyon",
            user_address = "5 avenue du Miel"
        };
        var updateResponse = _model.saveUser(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getUser();
        Assert.True(fetched.is_success);
        Assert.Equal("Pierre Apiculteur", fetched.returned_item!.user_name);
        Assert.Equal("0987654321", fetched.returned_item.user_phone);
        Assert.Equal("pierre@miel.com", fetched.returned_item.user_email);
        Assert.Equal("Lyon", fetched.returned_item.user_city);
    }

    // ─────────────────────────────────────────────
    //  getUser
    // ─────────────────────────────────────────────

    [Fact]
    public void GetUser_WhenNoUserExists_ReturnsNotFound()
    {
        var response = _model.getUser();

        Assert.False(response.is_success);
    }

    [Fact]
    public void GetUser_AfterInsert_ReturnsUser()
    {
        var item = new UserItem
        {
            user_name = "Jean Apiculteur",
            user_phone = "0123456789",
            user_email = "jean@miel.com",
            user_city = "Dijon",
            user_address = "3 rue des Abeilles"
        };
        _model.saveUser(item);

        var response = _model.getUser();

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Jean Apiculteur", response.returned_item.user_name);
        Assert.Equal("0123456789", response.returned_item.user_phone);
        Assert.Equal("jean@miel.com", response.returned_item.user_email);
        Assert.Equal("Dijon", response.returned_item.user_city);
        Assert.Equal("3 rue des Abeilles", response.returned_item.user_address);
    }

    // ─────────────────────────────────────────────
    //  saveUser – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveUser_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new UserItem { user_name = "Jean Apiculteur" };
        var response = _model.saveUser(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getUser();
        Assert.True(fetched.is_success);
        Assert.Equal("Jean Apiculteur", fetched.returned_item!.user_name);
    }

    [Fact]
    public void SaveUser_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new UserItem { user_name = "Jean Apiculteur" };
        var response = _model.saveUser(item);
        Assert.True(response.is_success);

        ABaseModel.rollbackTransaction();

        var fetched = _model.getUser();
        Assert.False(fetched.is_success);
    }
}
