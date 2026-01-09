using System.Windows.Controls;

namespace Mams_App.src.productsShapes;

/// <summary>
/// Interaction logic for ListProductShapePage.xaml
/// </summary>
public partial class ListProductShapePage : Page
{
    public ListProductShapePage()
    {
        InitializeComponent();
        DataContext = new ListProductShapeController();
    }
}
