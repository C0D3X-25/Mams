using Mams_App.src.fees;
using Mams_App.src.items;
using Mams_App.src.profits;
using System.Collections.ObjectModel;

namespace Mams_App.src.resumes;

public class ResumeItem : ABaseItem
{
    public ObservableCollection<ReceiptProfitDetailedItem> profit_items { get; set; } = [];
    public ObservableCollection<ReceiptFeeDetailedItem> fee_items { get; set; } = [];
}
