using Mams_App.src.items;

namespace Mams_App.src.doseUnits;

public class DoseUnitItem : ABaseItem
{
    public int dose_unit_id { get; set; } = 0;
    public string dose_unit_name { get; set; } = string.Empty;
    public string dose_unit_archive { get; set; } = string.Empty;
}
