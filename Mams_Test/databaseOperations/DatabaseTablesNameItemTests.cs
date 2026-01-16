using Mams_App.src.databaseOperations;

namespace Mams_Test.databaseOperations;

public class DatabaseTablesNameItemTests
{
    [Fact]
    public void DatabaseTablesNameItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new DatabaseTablesNameItem();

        // Assert
        Assert.Equal(EDatabaseTableName.NONE, item.m_name_in_database);
        Assert.Equal(string.Empty, item.m_name_to_display);
    }

    [Fact]
    public void DatabaseTablesNameItem_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new DatabaseTablesNameItem();

        // Act
        item.m_name_in_database = EDatabaseTableName.PRODUCT;
        item.m_name_to_display = "Products";

        // Assert
        Assert.Equal(EDatabaseTableName.PRODUCT, item.m_name_in_database);
        Assert.Equal("Products", item.m_name_to_display);
    }

    [Fact]
    public void DatabaseTablesNameItem_SetDifferentTableNames_ShouldReturnCorrectValues()
    {
        // Arrange
        var item = new DatabaseTablesNameItem();

        // Act & Assert
        item.m_name_in_database = EDatabaseTableName.ENTITY;
        Assert.Equal(EDatabaseTableName.ENTITY, item.m_name_in_database);

        item.m_name_in_database = EDatabaseTableName.CLIENT;
        Assert.Equal(EDatabaseTableName.CLIENT, item.m_name_in_database);

        item.m_name_in_database = EDatabaseTableName.SUPPLIER;
        Assert.Equal(EDatabaseTableName.SUPPLIER, item.m_name_in_database);

        item.m_name_in_database = EDatabaseTableName.BEEHIVE;
        Assert.Equal(EDatabaseTableName.BEEHIVE, item.m_name_in_database);
    }

    [Fact]
    public void DatabaseTablesNameItem_SetEmptyDisplayName_ShouldAllowEmptyString()
    {
        // Arrange
        var item = new DatabaseTablesNameItem { m_name_to_display = "Test" };

        // Act
        item.m_name_to_display = string.Empty;

        // Assert
        Assert.Equal(string.Empty, item.m_name_to_display);
    }
}

public class EDatabaseTableNameTests
{
    [Fact]
    public void EDatabaseTableName_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.Equal(0, (int)EDatabaseTableName.NONE);
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.ENTITY));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.CLIENT));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.SUPPLIER));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.PRODUCT));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.PRODUCT_CATEGORY));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.PRODUCT_SHAPE));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.PRODUCT_TYPE));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.PRODUCT_LOT));
        Assert.True(Enum.IsDefined(typeof(EDatabaseTableName), EDatabaseTableName.BEEHIVE));
    }

    [Fact]
    public void EDatabaseTableName_AllValues_ShouldBeUnique()
    {
        // Arrange
        var values = Enum.GetValues<EDatabaseTableName>();

        // Act
        var distinctCount = values.Distinct().Count();

        // Assert
        Assert.Equal(values.Length, distinctCount);
    }
}

public class EDeleteItemOperationTests
{
    [Fact]
    public void EDeleteItemOperation_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(EDeleteItemOperation), EDeleteItemOperation.SOFT_DELETE));
        Assert.True(Enum.IsDefined(typeof(EDeleteItemOperation), EDeleteItemOperation.HARD_DELETE));
        Assert.True(Enum.IsDefined(typeof(EDeleteItemOperation), EDeleteItemOperation.SAFE_DELETE));
        Assert.True(Enum.IsDefined(typeof(EDeleteItemOperation), EDeleteItemOperation.RESTORE));
        Assert.True(Enum.IsDefined(typeof(EDeleteItemOperation), EDeleteItemOperation.NONE));
    }

    [Fact]
    public void EDeleteItemOperation_AllValues_ShouldBeUnique()
    {
        // Arrange
        var values = Enum.GetValues<EDeleteItemOperation>();

        // Act
        var distinctCount = values.Distinct().Count();

        // Assert
        Assert.Equal(values.Length, distinctCount);
    }
}
