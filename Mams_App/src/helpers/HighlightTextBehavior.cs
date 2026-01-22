using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Mams_App.src.helpers;

/// <summary>
/// Attached behavior that highlights matching search patterns in a TextBlock.
/// The matching parts are displayed in bold.
/// </summary>
public static class HighlightTextBehavior
{
    #region Text Property

    /// <summary>
    /// Identifies the Text attached property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(HighlightTextBehavior),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// Gets the text to display with highlighting.
    /// </summary>
    public static string GetText(DependencyObject obj)
    {
        return (string)obj.GetValue(TextProperty);
    }

    /// <summary>
    /// Sets the text to display with highlighting.
    /// </summary>
    public static void SetText(DependencyObject obj, string value)
    {
        obj.SetValue(TextProperty, value);
    }

    #endregion

    #region HighlightText Property

    /// <summary>
    /// Identifies the HighlightText attached property (the search pattern to highlight).
    /// </summary>
    public static readonly DependencyProperty HighlightTextProperty =
        DependencyProperty.RegisterAttached(
            "HighlightText",
            typeof(string),
            typeof(HighlightTextBehavior),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// Gets the search pattern to highlight.
    /// </summary>
    public static string GetHighlightText(DependencyObject obj)
    {
        return (string)obj.GetValue(HighlightTextProperty);
    }

    /// <summary>
    /// Sets the search pattern to highlight.
    /// </summary>
    public static void SetHighlightText(DependencyObject obj, string value)
    {
        obj.SetValue(HighlightTextProperty, value);
    }

    #endregion

    /// <summary>
    /// Called when either Text or HighlightText property changes.
    /// Updates the TextBlock's Inlines collection with highlighted runs.
    /// </summary>
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock)
        {
            return;
        }

        string text = GetText(textBlock) ?? string.Empty;
        string highlightText = GetHighlightText(textBlock) ?? string.Empty;

        textBlock.Inlines.Clear();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (string.IsNullOrEmpty(highlightText))
        {
            // No search pattern, display plain text
            textBlock.Inlines.Add(new Run(text));
            return;
        }

        // Find all occurrences of the search pattern (case-insensitive)
        int currentIndex = 0;
        int searchIndex;

        while ((searchIndex = text.IndexOf(highlightText, currentIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            // Add the text before the match (normal weight)
            if (searchIndex > currentIndex)
            {
                textBlock.Inlines.Add(new Run(text[currentIndex..searchIndex]));
            }

            // Add the matching text (bold)
            textBlock.Inlines.Add(new Run(text.Substring(searchIndex, highlightText.Length))
            {
                FontWeight = FontWeights.Bold
            });

            currentIndex = searchIndex + highlightText.Length;
        }

        // Add any remaining text after the last match
        if (currentIndex < text.Length)
        {
            textBlock.Inlines.Add(new Run(text[currentIndex..]));
        }
    }
}
