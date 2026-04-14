using Mams_App.src.controllers;
using Mams_App.src.localizations;
using Mams_App.src.resumes;
using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.navigations;

public static class SPageNavigationController
{
    private const int MAX_NAVIGATION_HISTORY = 20;

    private static Frame? _m_frame;
    private static Page? _m_current_page;
    private static readonly Stack<Page> _m_back_stack = new();
    private static readonly Stack<Page> _m_forward_stack = new();

    /// <summary>
    /// Event raised when the current page changes. Provides the Type of the new page.
    /// </summary>
    public static event Action<Type?>? PageChanged;

    /// <summary>
    /// Initializes the application with the specified frame and navigates to the home page.
    /// </summary>
    /// <param name="frame">The <see cref="Frame"/> object used for navigation and rendering within the application.</param>
    public static void initialize(Frame frame)
    {
        _m_frame = frame;
        navigateToHomePage();
    }

    /// <summary>
    /// Navigates to the application's home page.
    /// </summary>
    public static void navigateToHomePage()
    {
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
    public static void navigateTo(Page page, bool compare_original = false)
    {
        if (_m_frame == null)
        {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page), "Page cannot be null.");
        }
        if (_m_current_page != null && _m_current_page.GetType() == page.GetType())
        {
            // If the current page is the same as the new page, do not navigate again.
            return;
        }

        // Check if current page's DataContext implements ICompareState and has unsaved changes
        if (compare_original)
        {
            if (_m_current_page?.DataContext is ICompareState compareState && !compareState.isStateOriginal())
            {
                MessageBoxResult result = MessageBox.Show(
                    Loc.Get("Common.UnsavedChangesMessage"),
                    Loc.Get("Common.Cancel"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                {
                    return; // Cancel navigation
                }
            }
        }

        // Push current page to back stack before navigating
        if (_m_current_page != null)
        {
            pushToStack(_m_back_stack, _m_current_page);
        }

        // Clear forward stack when navigating to a new page
        _m_forward_stack.Clear();

        _m_current_page = page;
        _m_frame?.Navigate(page);
        PageChanged?.Invoke(page.GetType());
    }

    /// <summary>
    /// Indicates whether there is a previous page to navigate back to.
    /// </summary>
    public static bool canNavigateBack() => _m_back_stack.Count > 0;

    /// <summary>
    /// Indicates whether there is a forward page to navigate to.
    /// </summary>
    public static bool canNavigateForward() => _m_forward_stack.Count > 0;

    /// <summary>
    /// Navigates to the previous page in the navigation stack.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static void navigateBack(bool compare_original = false)
    {
        if (_m_frame == null)
        {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (_m_back_stack.Count == 0)
        {
            return; // No previous page to navigate back to
        }

        // Check for unsaved changes before navigating
        if (compare_original)
        {
            if (_m_current_page?.DataContext is ICompareState compareState && !compareState.isStateOriginal())
            {
                MessageBoxResult result = MessageBox.Show(
                    Loc.Get("Common.UnsavedChangesMessage"),
                    Loc.Get("Common.Cancel"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                {
                    return; // Cancel navigation
                }
            }
        }

        // Push current page to forward stack
        if (_m_current_page != null)
        {
            pushToStack(_m_forward_stack, _m_current_page);
        }

        // Pop and navigate to previous page
        Page previous_page = _m_back_stack.Pop();

        _m_current_page = previous_page;
        _m_frame?.Navigate(previous_page);
        PageChanged?.Invoke(previous_page.GetType());

        // Refresh data if the page's DataContext supports it
        if (previous_page.DataContext is IRefreshable refreshable)
        {
            refreshable.refreshData();
        }
    }

    /// <summary>
    /// Navigates to the next page in the forward navigation stack.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static void navigateForward(bool compare_original = false)
    {
        if (_m_frame == null)
        {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (_m_forward_stack.Count == 0)
        {
            return; // No forward page to navigate to
        }

        // Check for unsaved changes before navigating
        if (compare_original)
        {
            if (_m_current_page?.DataContext is ICompareState compareState && !compareState.isStateOriginal())
            {
                MessageBoxResult result = MessageBox.Show(
                    Loc.Get("Common.UnsavedChangesMessage"),
                    Loc.Get("Common.Cancel"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                {
                    return; // Cancel navigation
                }
            }
        }

        // Push current page to back stack
        if (_m_current_page != null)
        {
            pushToStack(_m_back_stack, _m_current_page);
        }

        // Pop and navigate to forward page
        Page forward_page = _m_forward_stack.Pop();

        _m_current_page = forward_page;
        _m_frame?.Navigate(forward_page);
        PageChanged?.Invoke(forward_page.GetType());
    }

    /// <summary>
    /// Refreshes the current page by creating a new instance of it, reloading all data.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the frame is not initialized or no current page exists.</exception>
    public static void refreshCurrentPage()
    {
        if (_m_frame == null)
        {
            throw new InvalidOperationException("Frame is not initialized. Call initialize() first.");
        }
        if (_m_current_page == null)
        {
            throw new InvalidOperationException("No current page to refresh.");
        }

        // Create a new instance of the current page type to ensure latest data is loaded
        Type current_page_type = _m_current_page.GetType();
        Page? new_page = (Page?)Activator.CreateInstance(current_page_type);
        if (new_page == null)
        {
            throw new InvalidOperationException($"Failed to create an instance of the current page type: {current_page_type.FullName}");
        }

        // Reset current page to allow navigation to same type
        _m_current_page = null;
        navigateTo(new_page);
    }

    /// <summary>
    /// Pushes a page to the specified stack, removing the oldest entry if the stack exceeds the maximum size.
    /// </summary>
    private static void pushToStack(Stack<Page> stack, Page page)
    {
        if (stack.Count >= MAX_NAVIGATION_HISTORY)
        {
            // Remove oldest entries to make room
            var tempList = stack.ToList();
            tempList.RemoveAt(tempList.Count - 1); // Remove oldest (bottom of stack)
            stack.Clear();
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                stack.Push(tempList[i]);
            }
        }
        stack.Push(page);
    }
}
