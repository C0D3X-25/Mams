namespace Mams.src.models;

/// <summary>
/// Base class for all models to inherit from.
/// Holds the SQL connection object for all models to use.
/// </summary>
public abstract class ABaseModel {
    public readonly SQLConnectionModel _m_conn = new();
}
