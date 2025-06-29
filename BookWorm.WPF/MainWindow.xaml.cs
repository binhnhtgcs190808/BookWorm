using System.Windows;
using BookWorm.WPF.ViewModels;

namespace BookWorm.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}