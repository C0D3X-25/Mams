using System.Windows.Controls;

namespace Mams_App.src.productsShapes;

/// <summary>
/// Interaction logic for SaveProductShapePage.xaml
/// </summary>
public partial class SaveProductShapePage : Page
{

    public SaveProductShapePage(int id_to_load = 0)
    {

        InitializeComponent();
        DataContext = new SaveProductShapeController(id_to_load);
    }
}
