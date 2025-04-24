using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.clients;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveClientPage : Page {

    private readonly int _m_id_client = 0;

    public SaveClientPage(int id_client = 0) {
        InitializeComponent();
        this.Loaded += pageLoaded;
        this._m_id_client = id_client;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveClientController controller = new(_m_window.m_page_navigation, _m_id_client);
            this.DataContext = controller;
        }
    }
}
