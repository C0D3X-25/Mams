namespace Mams_Test.integration;

/// <summary>
/// Defines the xUnit test collection that shares a single <see cref="DatabaseFixture"/>
/// across all integration test classes.
/// 
/// Any test class decorated with <c>[Collection("Database")]</c> will receive the
/// shared fixture instance and will NOT run in parallel with other classes in the
/// same collection (ensuring no static-state conflicts in <c>ABaseModel</c>).
/// </summary>
[CollectionDefinition("Database")]
public class DatabaseCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
    // This class has no code — it only serves as an anchor for the collection definition.
}
