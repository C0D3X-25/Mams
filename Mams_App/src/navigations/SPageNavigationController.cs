using Mams.src.controllers;
using Mams.src.resumes;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.navigations;

public static class SPageNavigationController {

    private static Frame? _m_frame;
    private static Page? _m_current_page;
    private static Page? _m_previous_page;

    /// <summary>
    /// Initializes the application with the specified frame and navigates to the home page.
    /// </summary>
    /// <param name="frame">The <see cref="Frame"/> object used for navigation and rendering within the application.</param>
    public static void initialize(Frame frame) {
        _m_frame = frame;
        navigateToHomePage();
    }

    /// <summary>
    /// Navigates to the application's home page.
    /// </summary>
    public static void navigateToHomePage() {
        navigateTo(new ResumePage());
    }

    /// <summary>
    /// Navigates to the specified page, optionally checking for unsaved changes on the current page.
    /// </summary>
    /// <remarks>If the specified <paramref name="page"/> is of the same type as the current page, no
    /// navigation occurs.  When <paramref name="compare_original"/> is <see langword="true"/>, the method checks if the
    /// current page's  <c>DataContext</c> implements <c>ICompareState</c> and has unsaved changes. If unsaved changes
    /// are detected,  the user is prompted to confirm navigation. If the user chooses not to proceed, navigation is
    /// canceled.</remarks>
    /// <param name="page">The target <see cref="Page"/> to navigate to. Cannot be <see langword="null"/>.</param>
    /// <param name="compare_original">A <see cref="bool"/> value indicating whether to check for unsaved changes on the current page before
    /// navigating.  If <see langword="true"/>, the method prompts the user to confirm navigation if the current page
    /// has unsaved changes.</param>
    /// <exception cref="InvalidOperationException">Thrown if the navigation frame is not initialized. Ensure that <c>initialize()</c> is called before invoking
    /// this method.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="page"/> is <see langword="null"/>.</exception>
    public static void navigateTo(Page page, bool compare_original = false) {
        if (_m_frame == null) {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (page == null) {
            throw new ArgumentNullException(nameof(page), "Page cannot be null.");
        }
        if (_m_current_page != null && _m_current_page.GetType() == page.GetType()) {
            // If the current page is the same as the new page, do not navigate again.
            return;
        }

        // Check if current page's DataContext implements ICompareState and has unsaved changes
        if (compare_original) {
            if (_m_current_page?.DataContext is ICompareState compareState && !compareState.isStateOriginal()) {
                MessageBoxResult result = MessageBox.Show(
                    "En quittant la page, toutes les données modifiées seront perdues. Voulez-vous continuer?",
                    "Annuler",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No) {
                    return; // Cancel navigation
                }
            }
        }

        _m_previous_page = _m_current_page;
        _m_current_page = page;
        _m_frame?.Navigate(page);
    }

    /// <summary>
    /// Navigates to the previous page in the navigation stack.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static void navigateBack(bool compare_original = false) {
        if (_m_frame == null) {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (_m_previous_page == null) {
            throw new InvalidOperationException("No previous page to navigate back to.");
        }
        
        // Create a new instance of the previous page type to ensure latest data is loaded
        Type previous_page_type = _m_previous_page.GetType();
        if (previous_page_type == null) {
            throw new InvalidOperationException("Previous page type is null.");
        }
        Page? new_page = (Page?)Activator.CreateInstance(previous_page_type);
        if (new_page == null) {
            throw new InvalidOperationException($"Failed to create an instance of the previous page type: {previous_page_type.FullName}");
        }

        navigateTo(new_page, compare_original);
    }
}
