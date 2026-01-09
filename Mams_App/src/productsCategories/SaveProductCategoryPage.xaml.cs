using System.Windows.Controls;

namespace Mams_App.src.productsCategories;

/// <summary>
/// Interaction logic for SaveProductCategoryPage.xaml
/// </summary>
public partial class SaveProductCategoryPage : Page
{

    public SaveProductCategoryPage(int id_to_load = 0)
    {

        InitializeComponent();
        DataContext = new SaveProductCategoryController(id_to_load);
    }
}
