using Mams_App.src.items;

namespace Mams_App.src.suppliers;

public class SupplierItem : ABaseItem
{

    public int supplier_id { get; set; } = 0;
    public int fk_entity_id { get; set; } = 0;
}
