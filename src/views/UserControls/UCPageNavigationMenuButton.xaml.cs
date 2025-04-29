//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

//namespace Mams.src.views.userControls;

///// <summary>
///// Interaction logic for UCPageNavigationMenuButton.xaml
///// </summary>
//public partial class UCPageNavigationMenuButton : UserControl {
//    public new static readonly DependencyProperty ContentProperty =
//        DependencyProperty.Register("Content", typeof(object), typeof(UCPageNavigationMenuButton),
//            new PropertyMetadata(null, OnContentChanged));

//    public static readonly DependencyProperty CommandProperty =
//        DependencyProperty.Register("Command", typeof(ICommand), typeof(UCPageNavigationMenuButton),
//            new PropertyMetadata(null, OnCommandChanged));

//    public new object Content {
//        get => GetValue(ContentProperty);
//        set => SetValue(ContentProperty, value);
//    }

//    public ICommand Command {
//        get => (ICommand)GetValue(CommandProperty);
//        set => SetValue(CommandProperty, value);
//    }

//    public UCPageNavigationMenuButton() {
//        InitializeComponent();
//    }

//    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//        if (d is UCPageNavigationMenuButton control && control.menu_button != null) {
//            control.menu_button.Content = e.NewValue;
//        }
//    }

//    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//        if (d is UCPageNavigationMenuButton control && control.menu_button != null) {
//            control.menu_button.Command = e.NewValue as ICommand;
//        }
//    }
//}
