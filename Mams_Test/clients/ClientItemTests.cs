using Mams_App.src.clients;

namespace MamsTest.clients;

public class ClientItemTests
{
    [Fact]
    public void ClientItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ClientItem();

        // Assert
        Assert.Equal(0, item.client_id);
        Assert.Equal(0, item.fk_entity_id);
    }

    [Fact]
    public void ClientItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new ClientItem();

        // Act
        item.client_id = 1;
        item.fk_entity_id = 5;

        // Assert
        Assert.Equal(1, item.client_id);
        Assert.Equal(5, item.fk_entity_id);
    }

    [Fact]
    public void ClientItem_SetNegativeId_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ClientItem();

        // Act
        item.client_id = -1;

        // Assert
        Assert.Equal(-1, item.client_id);
    }

    [Fact]
    public void ClientItem_SetForeignKeyEntity_ShouldAllowZeroValue()
    {
        // Arrange
        var item = new ClientItem { fk_entity_id = 5 };

        // Act
        item.fk_entity_id = 0;

        // Assert
        Assert.Equal(0, item.fk_entity_id);
    }

    [Fact]
    public void ClientItem_SetNegativeForeignKey_ShouldAllowNegativeValue()
    {
        // Arrange
        var item = new ClientItem();

        // Act
        item.fk_entity_id = -1;

        // Assert
        Assert.Equal(-1, item.fk_entity_id);
    }
}
