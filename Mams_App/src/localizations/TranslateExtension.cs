using System.Windows.Markup;

namespace Mams_App.src.localizations;

/// <summary>
/// XAML Markup Extension for localization.
/// Allows using localized strings directly in XAML.
/// </summary>
/// <example>
/// <code>
/// &lt;TextBlock Text="{loc:Translate Page.ListFees}"/&gt;
/// &lt;Button Content="{loc:Translate Key=Common.Save}"/&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(string))]
public class TranslateExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the localization key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
    /// </summary>
    public TranslateExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateExtension"/> class with the specified key.
    /// </summary>
    /// <param name="key">The localization key.</param>
    public TranslateExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Returns the localized string for the specified key.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The localized string, or the key in brackets if not found.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
        {
            return string.Empty;
        }

        return Loc.Get(Key);
    }
}
