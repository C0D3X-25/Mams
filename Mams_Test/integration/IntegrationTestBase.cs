using Mams_App.src.models;

namespace Mams_Test.integration;

/// <summary>
/// Base class for all integration tests that interact with the database.
/// Ensures each test starts with a clean database and no leftover transaction state.
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly DatabaseFixture _fixture;

    protected IntegrationTestBase(DatabaseFixture fixture)
    {
        _fixture = fixture;

        // Ensure no leftover transaction from a previous failed test
        // Use while-loop to fully unwind nested transactions (depth > 1)
        while (ABaseModel.isTransactionActive())
        {
            ABaseModel.rollbackTransaction();
        }

        // Clean all tables before each test
        _fixture.cleanAllTables();
    }

    public void Dispose()
    {
        // Safety net: rollback any uncommitted transaction left by the test
        while (ABaseModel.isTransactionActive())
        {
            ABaseModel.rollbackTransaction();
        }

        GC.SuppressFinalize(this);
    }
}
