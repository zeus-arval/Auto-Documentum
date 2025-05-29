using Soft.MVVM;
using System.Windows;

namespace Soft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; init; }
        public MainWindow(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;

            InitializeComponent();
        }
    }
}