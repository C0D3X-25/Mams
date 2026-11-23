namespace Mams_App.src.controllers;

/// <summary>
/// Interface for controllers that support refreshing their data, typically used when navigating back to a page.
/// </summary>
internal interface IRefreshable
{
    void refreshData();
}
