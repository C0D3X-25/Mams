using Mams.src.fees;
using Mams.src.items;
using Mams.src.profits;
using System.Collections.ObjectModel;

namespace Mams.src.resumes;

public class ResumeItem : ABaseItem {
    public ObservableCollection<ReceiptProfitDetailedItem> profit_items { get; set; } = [];
    public ObservableCollection<ReceiptFeeDetailedItem> fee_items { get; set; } = [];
}
