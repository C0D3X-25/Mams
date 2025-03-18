using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mams.views.userControls;

/// <summary>
/// Interaction logic for UCAbortSaveButtons.xaml
/// </summary>
public partial class UCAbortSaveButtons : UserControl {



    public string label_left_button {
        get { return (string)GetValue(label_left_buttonProperty); }
        set { SetValue(label_left_buttonProperty, value); }
    }

    // Using a DependencyProperty as the backing store for label_left_button.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty label_left_buttonProperty =
        DependencyProperty.Register("label_left_button", typeof(string), typeof(UCAbortSaveButtons), new PropertyMetadata("Annuler"));

    public string label_right_button {
        get { return (string)GetValue(label_right_buttonProperty); }
        set { SetValue(label_right_buttonProperty, value); }
    }

    // Using a DependencyProperty as the backing store for label_right_button.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty label_right_buttonProperty =
        DependencyProperty.Register("label_right_button", typeof(string), typeof(UCAbortSaveButtons), new PropertyMetadata("Enregistrer"));


    public UCAbortSaveButtons()
    {
        InitializeComponent();
    }
}
