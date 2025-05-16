using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

public class ListFeeItem : ABaseItem {
    ObservableCollection<FeeItem> fee_items { get; set; } = new();
}
