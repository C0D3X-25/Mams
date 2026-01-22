using Mams_App.src.databaseOperations;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Mams_App.src.searches;

/// <summary>
/// Model for searching across multiple database tables.
/// </summary>
public class SearchModel : ABaseModel
{
    /// <summary>
    /// Searches all relevant tables for items matching the search text.
    /// Searches by name, custom ID (for fees/profits), and date.
    /// </summary>
    /// <param name="search_text">The text to search for.</param>
    /// <returns>A response containing the list of matching search items.</returns>
    public ResponseGetAllItems<SearchItem> searchAllItems(string search_text)
    {
        if (string.IsNullOrWhiteSpace(search_text))
        {
            return ResponseGetAllItems<SearchItem>.Success([]);
        }

        return executeWithConnection(connection =>
        {
            try
            {
                var results = new ObservableCollection<SearchItem>();
                string search_pattern = $"%{search_text}%";

                // Search Products
                searchProducts(connection, search_pattern, results);

                // Search Entities
                searchEntities(connection, search_pattern, results);

                // Search Beehives
                searchBeehives(connection, search_pattern, results);

                // Search Product Lots
                searchProductLots(connection, search_pattern, results);

                // Search Product Types
                searchProductTypes(connection, search_pattern, results);

                // Search Product Categories
                searchProductCategories(connection, search_pattern, results);

                // Search Product Shapes
                searchProductShapes(connection, search_pattern, results);

                // Search Fees (receipts from suppliers)
                searchFees(connection, search_pattern, search_text, results);

                // Search Profits (receipts from clients)
                searchProfits(connection, search_pattern, search_text, results);

                return ResponseGetAllItems<SearchItem>.Success(results);
            }
            catch (Exception ex)
            {
                return ResponseGetAllItems<SearchItem>.Failure(
                    errors.EErrors.DATABASE_CONNECTION,
                    $"SearchModel.searchAllItems: Error searching items - {ex.Message}");
            }
        });
    }

    /// <summary>
    /// Searches the products table for items matching the search pattern.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProducts(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT product_id, product_name, product_archive
            FROM products
            WHERE product_name LIKE @search
            AND (product_archive IS NULL OR product_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the entities table for contacts matching the search pattern.
    /// Matches against entity name and city.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchEntities(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT entity_id, entity_name, entity_city, entity_archive
            FROM entities
            WHERE (entity_name LIKE @search OR entity_city LIKE @search)
            AND (entity_archive IS NULL OR entity_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the beehives table for beehives matching the search pattern.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchBeehives(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT beehive_id, beehive_name, beehive_archive
            FROM beehives
            WHERE beehive_name LIKE @search
            AND (beehive_archive IS NULL OR beehive_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the product lots table for lots matching the search pattern.
    /// Includes year and associated beehive information in the details.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProductLots(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT pl.product_lot_id, pl.product_lot_name, pl.product_lot_year, b.beehive_name, pl.product_lot_archive
            FROM products_lots pl
            LEFT JOIN beehives b ON pl.fk_beehive_id = b.beehive_id
            WHERE pl.product_lot_name LIKE @search
            AND (pl.product_lot_archive IS NULL OR pl.product_lot_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the product types table for types matching the search pattern.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProductTypes(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT product_type_id, product_type_name, product_type_archive
            FROM products_types
            WHERE product_type_name LIKE @search
            AND (product_type_archive IS NULL OR product_type_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the product categories table for categories matching the search pattern.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProductCategories(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT product_category_id, product_category_name, product_category_archive
            FROM products_categories
            WHERE product_category_name LIKE @search
            AND (product_category_archive IS NULL OR product_category_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the product shapes table for shapes matching the search pattern.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProductShapes(MySqlConnection connection, string search_pattern, ObservableCollection<SearchItem> results)
    {
        string query = @"
            SELECT product_shape_id, product_shape_name, product_shape_archive
            FROM products_shapes
            WHERE product_shape_name LIKE @search
            AND (product_shape_archive IS NULL OR product_shape_archive != '1901-01-01')";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the fees (supplier receipts) for items matching the search pattern.
    /// Matches against receipt number, supplier entity name, or date.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="search_text">The original search text (for date parsing).</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchFees(MySqlConnection connection, string search_pattern, string search_text, ObservableCollection<SearchItem> results)
    {
        // Search by receipt_number (custom ID), entity name, or date
        string query = @"
            SELECT r.receipt_id, r.receipt_number, r.receipt_date_created, r.receipt_total_price, e.entity_name
            FROM receipts r
            INNER JOIN receipts_suppliers rs ON r.receipt_id = rs.fk_receipt_id
            INNER JOIN suppliers s ON rs.fk_supplier_id = s.supplier_id
            INNER JOIN entities e ON s.fk_entity_id = e.entity_id
            WHERE (r.receipt_number LIKE @search 
                   OR e.entity_name LIKE @search
                   OR DATE_FORMAT(r.receipt_date_created, '%d.%m.%Y') LIKE @search)";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

    /// <summary>
    /// Searches the profits (client receipts) for items matching the search pattern.
    /// Matches against receipt number, client entity name, or date.
    /// </summary>
    /// <param name="connection">The active MySQL connection.</param>
    /// <param name="search_pattern">The SQL LIKE pattern to search for.</param>
    /// <param name="search_text">The original search text (for date parsing).</param>
    /// <param name="results">The collection to add matching items to.</param>
    private static void searchProfits(MySqlConnection connection, string search_pattern, string search_text, ObservableCollection<SearchItem> results)
    {
        // Search by receipt_number (custom ID), entity name, or date
        string query = @"
            SELECT r.receipt_id, r.receipt_number, r.receipt_date_created, r.receipt_total_price, e.entity_name
            FROM receipts r
            INNER JOIN receipts_clients rc ON r.receipt_id = rc.fk_receipt_id
            INNER JOIN clients c ON rc.fk_client_id = c.client_id
            INNER JOIN entities e ON c.fk_entity_id = e.entity_id
            WHERE (r.receipt_number LIKE @search 
                   OR e.entity_name LIKE @search
                   OR DATE_FORMAT(r.receipt_date_created, '%d.%m.%Y') LIKE @search)";

        using MySqlCommand cmd = new(query, connection);
        cmd.Parameters.AddWithValue("@search", search_pattern);

        using MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
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
    }

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
