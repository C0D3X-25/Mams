using Mams.src.resumes;
using System.Windows.Controls;

namespace Mams.src.navigations;

public static class SPageNavigationController {

    private static Frame? _m_frame;
    private static Page? _m_current_page;

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
    /// Navigates to the specified page within the current frame.
    /// </summary>
    /// <param name="page">The page to navigate to. Must not be null.</param>
    public static void navigateTo(Page page) {
        if (page == null) {
            throw new ArgumentNullException(nameof(page), "Page cannot be null.");
        }
        if (_m_frame == null) {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (_m_current_page != null && _m_current_page.GetType() == page.GetType()) {
            // If the current page is the same as the new page, do not navigate again.
            return;
        }
        _m_current_page = page;
        _m_frame?.Navigate(page);
    }
}
