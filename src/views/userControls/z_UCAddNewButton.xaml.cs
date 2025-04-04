using Mams.src.controllers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCAddNewButton.xaml
/// </summary>
public partial class UCAddNewButton : UserControl {

    public UCAddNewButton() {
        InitializeComponent();

        this.Loaded += UCAddNew_Loaded; // need to wait for the window to be loaded
    }

    public string label_add_new_button {
        get { return (string)GetValue(label_add_new_buttonProperty); }
        set { SetValue(label_add_new_buttonProperty, value); }
    }
    public static readonly DependencyProperty label_add_new_buttonProperty =
        DependencyProperty.Register("label_add_new_button", typeof(string), typeof(UCAddNewButton), new PropertyMetadata("Ajouter"));

    public ICommand add_new_command {
        get { return (ICommand)GetValue(add_new_commandProperty); }
        set { SetValue(add_new_commandProperty, value); }
    }
    public static readonly DependencyProperty add_new_commandProperty =
        DependencyProperty.Register("add_new_command", typeof(ICommand), typeof(UCAddNewButton));


    private void UCAddNew_Loaded(object sender, RoutedEventArgs e) {
        MainWindow? main_window = Window.GetWindow(this) as MainWindow;

        if (main_window != null) {
            z_UCAddNewButtonController add_new_controller = new(main_window.m_page_navigation);
            this.DataContext = add_new_controller;
        }
    }
}
