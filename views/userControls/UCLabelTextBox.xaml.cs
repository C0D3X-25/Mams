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
/// Interaction logic for UCLabelTextBox.xaml
/// </summary>
public partial class UCLabelTextBox : UserControl {



    public string label {
        get { return (string)GetValue(labelProperty); }
        set { SetValue(labelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for label.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty labelProperty =
        DependencyProperty.Register("label", typeof(string), typeof(UCLabelTextBox), new PropertyMetadata("label to change text"));



    public UCLabelTextBox()
    {
        InitializeComponent();
    }
}
