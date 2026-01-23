using Mams_App.src.databaseConnections;
using Mams_App.src.databaseOperations;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.models;
using MySqlConnector;

namespace Mams_App.src.searches;

/// <summary>
/// Model for searching across multiple database tables.
/// </summary>
public class SearchModel : ABaseModel
{
    /// <summary>
    /// Searches all relevant tables asynchronously with parallel queries and progressive results.
    /// </summary>
    /// <param name="search_text">The text to search for.</param>
    /// <param name="cancellationToken">Token to cancel the search operation.</param>
    /// <param name="onBatchCompleted">Callback invoked when each table search completes with its results.</param>
    public async Task searchAllItemsAsync(string search_text, CancellationToken cancellationToken, Action<List<SearchItem>> onBatchCompleted)
    {
        if (string.IsNullOrWhiteSpace(search_text))
        {
            return;
        }

        string search_pattern = $"%{search_text}%";

        // Define all search tasks - each will run in parallel with its own connection
        var searchTasks = new List<Task>
        {
            Task.Run(() => searchTableAsync("Products", search_pattern, searchProductsQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("Entities", search_pattern, searchEntitiesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("Beehives", search_pattern, searchBeehivesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("ProductLots", search_pattern, searchProductLotsQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("ProductTypes", search_pattern, searchProductTypesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("ProductCategories", search_pattern, searchProductCategoriesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("ProductShapes", search_pattern, searchProductShapesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("Fees", search_pattern, searchFeesQuery, cancellationToken, onBatchCompleted), cancellationToken),
            Task.Run(() => searchTableAsync("Profits", search_pattern, searchProfitsQuery, cancellationToken, onBatchCompleted), cancellationToken)
        };

        try
        {
            await Task.WhenAll(searchTasks);
        }
        catch (OperationCanceledException)
        {
            // Search was cancelled, this is expected
            throw;
        }
    }

    /// <summary>
    /// Executes a search query on a specific table asynchronously.
    /// </summary>
    private static async Task searchTableAsync(
        string tableName,
        string search_pattern,
        Func<MySqlConnection, string, CancellationToken, Task<List<SearchItem>>> queryFunc,
        CancellationToken cancellationToken,
        Action<List<SearchItem>> onBatchCompleted)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = SQLConnectionModel.GetConnection();
        
        try
        {
            var results = await queryFunc(connection, search_pattern, cancellationToken);
            
            if (results.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                onBatchCompleted(results);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // Log error but continue with other searches
        }
    }

    #region Async Query Methods

    private static async Task<List<SearchItem>> searchProductsQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT product_id, product_name, product_archive
            FROM products
            WHERE product_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var archiveDate = reader.getSafeValue<DateOnly?>("product_archive");
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("product_id"),
                item_type = "Product",
                item_name = reader.getSafeValue("product_name", string.Empty),
                item_details = getArchiveDetails(archiveDate)
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchEntitiesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT entity_id, entity_name, entity_city, entity_archive
            FROM entities
            WHERE (entity_name LIKE @search OR entity_city LIKE @search)";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var city = reader.getSafeValue("entity_city", string.Empty);
            var archiveDate = reader.getSafeValue<DateOnly?>("entity_archive");
            var archiveDetails = getArchiveDetails(archiveDate);
            var details = string.IsNullOrEmpty(archiveDetails) ? city : $"{city} - {archiveDetails}";

            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("entity_id"),
                item_type = "Entity",
                item_name = reader.getSafeValue("entity_name", string.Empty),
                item_details = details.TrimStart(' ', '-', ' ')
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchBeehivesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT beehive_id, beehive_name, beehive_archive
            FROM beehives
            WHERE beehive_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var archiveDate = reader.getSafeValue<DateOnly?>("beehive_archive");
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("beehive_id"),
                item_type = "Beehive",
                item_name = reader.getSafeValue("beehive_name", string.Empty),
                item_details = getArchiveDetails(archiveDate)
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchProductLotsQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT pl.product_lot_id, pl.product_lot_name, pl.product_lot_year, b.beehive_name, pl.product_lot_archive
            FROM products_lots pl
            LEFT JOIN beehives b ON pl.fk_beehive_id = b.beehive_id
            WHERE pl.product_lot_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var year = reader.getSafeValue("product_lot_year", 0);
            var beehive = reader.getSafeValue("beehive_name", string.Empty);
            var archiveDate = reader.getSafeValue<DateOnly?>("product_lot_archive");
            var archiveDetails = getArchiveDetails(archiveDate);
            var details = $"{year} - {beehive}";
            if (!string.IsNullOrEmpty(archiveDetails))
            {
                details += $" - {archiveDetails}";
            }

            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("product_lot_id"),
                item_type = "ProductLot",
                item_name = reader.getSafeValue("product_lot_name", string.Empty),
                item_details = details
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchProductTypesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT product_type_id, product_type_name, product_type_archive
            FROM products_types
            WHERE product_type_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var archiveDate = reader.getSafeValue<DateOnly?>("product_type_archive");
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("product_type_id"),
                item_type = "ProductType",
                item_name = reader.getSafeValue("product_type_name", string.Empty),
                item_details = getArchiveDetails(archiveDate)
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchProductCategoriesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT product_category_id, product_category_name, product_category_archive
            FROM products_categories
            WHERE product_category_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var archiveDate = reader.getSafeValue<DateOnly?>("product_category_archive");
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("product_category_id"),
                item_type = "ProductCategory",
                item_name = reader.getSafeValue("product_category_name", string.Empty),
                item_details = getArchiveDetails(archiveDate)
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchProductShapesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT product_shape_id, product_shape_name, product_shape_archive
            FROM products_shapes
            WHERE product_shape_name LIKE @search";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var archiveDate = reader.getSafeValue<DateOnly?>("product_shape_archive");
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("product_shape_id"),
                item_type = "ProductShape",
                item_name = reader.getSafeValue("product_shape_name", string.Empty),
                item_details = getArchiveDetails(archiveDate)
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchFeesQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT r.receipt_id, r.receipt_number, r.receipt_date_created, r.receipt_total_price, e.entity_name
            FROM receipts r
            INNER JOIN receipts_suppliers rs ON r.receipt_id = rs.fk_receipt_id
            INNER JOIN suppliers s ON rs.fk_supplier_id = s.supplier_id
            INNER JOIN entities e ON s.fk_entity_id = e.entity_id
            WHERE (r.receipt_number LIKE @search 
                   OR e.entity_name LIKE @search
                   OR DATE_FORMAT(r.receipt_date_created, '%d.%m.%Y') LIKE @search)";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var date = reader.getSafeValue("receipt_date_created", DateOnly.MinValue);
            var total = reader.getSafeValue("receipt_total_price", 0.0M);
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("receipt_id"),
                item_type = "Fee",
                item_name = reader.getSafeValue("entity_name", string.Empty),
                item_custom_id = reader.getSafeValue("receipt_number", string.Empty),
                item_date_value = date,
                item_details = $"Total: {total:C}"
            });
        }
        return results;
    }

    private static async Task<List<SearchItem>> searchProfitsQuery(MySqlConnection connection, string search_pattern, CancellationToken ct)
    {
        var results = new List<SearchItem>();
        string query = @"
            SELECT r.receipt_id, r.receipt_number, r.receipt_date_created, r.receipt_total_price, e.entity_name
            FROM receipts r
            INNER JOIN receipts_clients rc ON r.receipt_id = rc.fk_receipt_id
            INNER JOIN clients c ON rc.fk_client_id = c.client_id
            INNER JOIN entities e ON c.fk_entity_id = e.entity_id
            WHERE (r.receipt_number LIKE @search 
                   OR e.entity_name LIKE @search
                   OR DATE_FORMAT(r.receipt_date_created, '%d.%m.%Y') LIKE @search)";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var date = reader.getSafeValue("receipt_date_created", DateOnly.MinValue);
            var total = reader.getSafeValue("receipt_total_price", 0.0M);
            results.Add(new SearchItem
            {
                item_id = reader.getSafeValue<int>("receipt_id"),
                item_type = "Profit",
                item_name = reader.getSafeValue("entity_name", string.Empty),
                item_custom_id = reader.getSafeValue("receipt_number", string.Empty),
                item_date_value = date,
                item_details = $"Total: {total:C}"
            });
        }
        return results;
    }

    #endregion

    /// <summary>
    /// Returns a localized "Archived: {date}" string if the item is archived, otherwise returns empty string.
    /// </summary>
    /// <param name="archiveDate">The archive date, or null if not archived.</param>
    /// <returns>A formatted archive details string, or empty string if not archived.</returns>
    private static string getArchiveDetails(DateOnly? archiveDate)
    {
        if (archiveDate == null || archiveDate == DateOnly.MinValue || archiveDate == new DateOnly(1901, 1, 1))
        {
            return string.Empty;
        }

        return $"{Loc.Get("Label.Archived")}: {archiveDate:d}";
    }
}
