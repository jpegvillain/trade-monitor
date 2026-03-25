using System.Windows;
using TradeMonitor.App.ViewModels;

namespace TradeMonitor.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}