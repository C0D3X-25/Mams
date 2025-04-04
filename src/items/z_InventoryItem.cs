using Mams.src.helpers;

namespace Mams.src.items;
   
public class z_InventoryItem : ABaseItem {

    public int m_id { get; private set; } = 0;
    public string m_name { get; private set; } = string.Empty;
    public int m_quantity { get; private set; } = 0;

    public z_InventoryItem(int id, string name, int quantity) {
        m_id = ConvertTypes.tryConvertToID(id);
        m_name = name;
        m_quantity = ConvertTypes.tryConvertToIntegerMoreThanZero(quantity);
    }

    public z_InventoryItem(int id, string name, string quantity) {
        m_id = ConvertTypes.tryConvertToID(id);
        m_name = name;
        m_quantity = ConvertTypes.tryConvertToIntegerMoreThanZero(quantity);
    }

    public z_InventoryItem(string id, string name, int quantity) {
        m_id = ConvertTypes.tryConvertToID(id);
        m_name = name;
        m_quantity = ConvertTypes.tryConvertToIntegerMoreThanZero(quantity);
    }

    public z_InventoryItem(string id, string name, string quantity) {
        m_id = ConvertTypes.tryConvertToID(id);
        m_name = name;
        m_quantity = ConvertTypes.tryConvertToIntegerMoreThanZero(quantity);
    }
}
