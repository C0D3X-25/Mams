using Mams.src.fees;
using Mams.src.items;
using Mams.src.profits;
using System.Collections.ObjectModel;

namespace Mams.src.resumes;

public class ResumeItem : ABaseItem {
    public ObservableCollection<ProfitItem> profit_items { get; set; } = new();
    public ObservableCollection<FeeReceiptItem> fee_items { get; set; } = new();
}
