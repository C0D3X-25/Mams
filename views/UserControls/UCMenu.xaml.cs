using Mams.src.ctrl;
using Mams.views.pages;
using System.Windows;
using System.Windows.Controls;

namespace Mams.views.userControls;

/// <summary>
/// Interaction logic for UCMenu.xaml
/// </summary>
public partial class UCMenu : UserControl {

    private readonly MainWindow? _m_window;
    public UCMenu() {
        InitializeComponent();

        _m_window = Window.GetWindow(this) as MainWindow;

        pageNav_clients_btn.Click += (sender, e) => {
            _m_window?.m_page_navigation.navigateToPage(new ListClientPage());
        };
    }
}
