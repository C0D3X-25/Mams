using Mams_App.src.items;

namespace Mams_App.src.treatments;

public class TreatmentItem : ABaseItem
{
    public int treatment_id { get; set; } = 0;
    public string treatment_date { get; set; } = string.Empty;
    public int treatment_hive_count { get; set; } = 0;
    public decimal treatment_dose_per_hive { get; set; } = 0;
    public int fk_beehive_id { get; set; } = 0;
    public int fk_treatment_stock_id { get; set; } = 0;

    // Joined fields (not stored in treatments table)
    public string beehive_name { get; set; } = string.Empty;
    public string beehive_number { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public string region_name { get; set; } = string.Empty;
    public string dose_unit_name { get; set; } = string.Empty;
    public string supplier_name { get; set; } = string.Empty;
    public decimal stock_initial_quantity { get; set; } = 0;
    public decimal stock_remaining_quantity { get; set; } = 0;

    // Calculated field
    public decimal treatment_dose_total => treatment_hive_count * treatment_dose_per_hive;
}
