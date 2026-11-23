using Mams_App.src.items;

namespace Mams_App.src.treatmentStocks;

public class TreatmentStockItem : ABaseItem
{
    public int treatment_stock_id { get; set; } = 0;
    public string treatment_stock_purchase_date { get; set; } = string.Empty;
    public decimal treatment_stock_initial_quantity { get; set; } = 0;
    public string treatment_stock_first_used_date { get; set; } = string.Empty;
    public string treatment_stock_last_used_date { get; set; } = string.Empty;
    public int fk_product_id { get; set; } = 0;
    public int fk_dose_unit_id { get; set; } = 0;
    public int fk_supplier_id { get; set; } = 0;

    // Joined fields (not stored in treatment_stocks table)
    public string product_name { get; set; } = string.Empty;
    public string dose_unit_name { get; set; } = string.Empty;
    public string supplier_name { get; set; } = string.Empty;

    // Calculated field: set by the model after summing usages
    public decimal treatment_stock_used_quantity { get; set; } = 0;
    public decimal treatment_stock_remaining_quantity => treatment_stock_initial_quantity - treatment_stock_used_quantity;

    /// <summary>
    /// Display string for ComboBox selection in the treatment form.
    /// </summary>
    public string display_name => $"{product_name} - {supplier_name} ({treatment_stock_remaining_quantity:F2} {dose_unit_name})";
}
