using Org.BouncyCastle.Crypto.Modes.Gcm;
using Project_Mams.src.items;

namespace Project_Mams.src.items;
   
public class InventoryItem : ABaseItem {
    public int m_id { get; private set; } = 0;
    public string m_name { get; private set; } = string.Empty;
    public int m_quantity { get; private set; } = 0;

    public InventoryItem(int id, string name, int quantity) {
        m_id = tryConvertToID(id);
        m_name = name;
        m_quantity = tryConvertToIntegerMoreThanZero(quantity);
    }

    public InventoryItem(int id, string name, string quantity) {
        m_id = tryConvertToID(id);
        m_name = name;
        m_quantity = tryConvertToIntegerMoreThanZero(quantity);
    }

    public InventoryItem(string id, string name, int quantity) {
        m_id = tryConvertToID(id);
        m_name = name;
        m_quantity = tryConvertToIntegerMoreThanZero(quantity);
    }

    public InventoryItem(string id, string name, string quantity) {
        m_id = tryConvertToID(id);
        m_name = name;
        m_quantity = tryConvertToIntegerMoreThanZero(quantity);
    }
}
