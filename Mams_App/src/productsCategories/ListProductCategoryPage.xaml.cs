using System.Windows.Controls;

namespace Mams_App.src.productsCategories;

/// <summary>
/// Interaction logic for ListCategoryPage.xaml
/// </summary>
public partial class ListProductCategoryPage : Page {
    public ListProductCategoryPage() { 
        InitializeComponent();
        DataContext = new ListProductCategoryController();
    }
}
