using Mams.src.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.receipts;

public class ReceiptProductItem : ABaseItem {
    public int receipt_product_id { get; set; } = 0;
    public int receipt_product_quantity { get; set; } = 0;
    public double receipt_product_unity_price { get; set; } = 0.0;
    public int fk_product_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
}
