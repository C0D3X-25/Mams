using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.mainWindow.userControls;

/// <summary>
/// Interaction logic for UCLabelTextBox.xaml
/// </summary>
public partial class UCLabelTextBox : UserControl
{

    public UCLabelTextBox()
    {
        InitializeComponent();
    }


    public string label
    {
        get { return (string)GetValue(labelProperty); }
        set { SetValue(labelProperty, value); }
    }
    public static readonly DependencyProperty labelProperty =
        DependencyProperty.Register("label", typeof(string), typeof(UCLabelTextBox),
            new PropertyMetadata("use 'label' to change text"));


    public string text
    {
        get { return (string)GetValue(textProperty); }
        set { SetValue(textProperty, value); }
    }
    public static readonly DependencyProperty textProperty =
        DependencyProperty.Register("text", typeof(string), typeof(UCLabelTextBox),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.Journal
            )
        );


    public bool isEnabled
    {
        get { return (bool)GetValue(isEnabledProperty); }
        set { SetValue(isEnabledProperty, value); }
    }
    public static readonly DependencyProperty isEnabledProperty =
        DependencyProperty.Register("isEnabled", typeof(bool), typeof(UCLabelTextBox), new PropertyMetadata(true));


}
