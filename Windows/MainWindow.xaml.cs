using ImageTool.Stores.Interfaces;
using ImageTool.Views.Main;
using System.Windows;

namespace ImageTool.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(INavigationStore navigationStore)
        {
            InitializeComponent();
            userControl.Content = navigationStore.GetUserControl<SearchMainUC>();
        }
    }
}