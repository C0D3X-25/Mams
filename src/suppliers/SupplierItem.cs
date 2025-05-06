using Mams.src.items;

namespace Mams.src.suppliers;

public class SupplierItem : ABaseItem {

    public int supplier_id { get; set; } = 0;
    public int fk_entity_id { get; set; } = 0;
}
