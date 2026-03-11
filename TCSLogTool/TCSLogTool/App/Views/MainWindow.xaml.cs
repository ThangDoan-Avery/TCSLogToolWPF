using System.Windows;
using TCSLogTool.App.ViewModels;

namespace TCSLogTool.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }
}