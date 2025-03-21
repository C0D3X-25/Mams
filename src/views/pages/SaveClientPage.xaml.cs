using Mams.src.controllers;
using System.Windows.Controls;

namespace Mams.src.views.pages;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveClientPage : Page {

    public SaveClientPage() {
        InitializeComponent();
        this.DataContext = new SaveClientController();
    }
}
