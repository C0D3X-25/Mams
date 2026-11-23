using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mams_App.src.helpers;

/// <summary>
/// Provides generic sorting functionality for ListView/GridView columns.
/// </summary>
public static class SortHelper
{
    /// <summary>
    /// Result of a sort operation containing the new column name and direction.
    /// </summary>
    public readonly record struct SortResult(string ColumnName, ListSortDirection Direction);

    /// <summary>
    /// Sorts a collection by the specified GridViewColumn, toggling direction if the same column is clicked again.
    /// </summary>
    /// <param name="parameter">The GridViewColumn passed from the command.</param>
    /// <param name="collection">The collection to sort.</param>
    /// <param name="currentSortedColumn">The currently sorted column name.</param>
    /// <param name="currentSortDirection">The current sort direction.</param>
    /// <returns>A SortResult with the new column name and direction, or null if sorting could not be applied.</returns>
    public static SortResult? sortByColumn(
        object? parameter,
        IEnumerable? collection,
        string currentSortedColumn,
        ListSortDirection currentSortDirection)
    {
        if (parameter is not GridViewColumn column || collection == null)
            return null;

        // Use SortPath attached property if available, otherwise use DisplayMemberBinding path
        string? columnName = SortPathAttached.GetSortPath(column);
        if (string.IsNullOrEmpty(columnName) && column.DisplayMemberBinding is Binding binding)
        {
            columnName = binding.Path.Path;
        }

        if (string.IsNullOrEmpty(columnName))
            return null;

        ListSortDirection direction = ListSortDirection.Ascending;

        if (currentSortedColumn == columnName)
        {
            direction = currentSortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
        }

        ICollectionView view = CollectionViewSource.GetDefaultView(collection);
        view.SortDescriptions.Clear();
        view.SortDescriptions.Add(new SortDescription(columnName, direction));

        return new SortResult(columnName, direction);
    }
}
