using System.Windows;

namespace BookWorm.WPF;

/// Interaction logic for MainWindow.xaml
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // The DataContext is now set in App.xaml.cs,
        // which allows for dependency injection into the ViewModel.
    }
}