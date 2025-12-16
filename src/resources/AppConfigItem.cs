namespace Mams.src.resources;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public class AppConfigItem
{
    /// <summary>
    /// Gets or sets the window left position.
    /// </summary>
    public double WindowLeft { get; set; } = 100;

    /// <summary>
    /// Gets or sets the window top position.
    /// </summary>
    public double WindowTop { get; set; } = 100;

    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    public double WindowWidth { get; set; } = 1224;

    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    public double WindowHeight { get; set; } = 800;

    /// <summary>
    /// Gets or sets whether the window is maximized.
    /// </summary>
    public bool IsMaximized { get; set; } = false;
}
