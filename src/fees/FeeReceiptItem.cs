using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

// class representing a fee receipt item
public class FeeReceiptItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public int fk_supplier_id { get; set; } = 0;
    public string fk_supplier_name { get; set; } = string.Empty;
    public decimal receipt_total_price { get; set; } = 0.0M;
    public string receipt_date_created { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
    public ObservableCollection<FeeProductItem> m_fee_items = new();
}
