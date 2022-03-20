using ImageTool.Stores.Interfaces;
using ImageTool.Views.Management;
using System.Windows;

namespace ImageTool.Windows
{
    /// <summary>
    /// Interaction logic for ManagementWindow.xaml
    /// </summary>
    public partial class ManagementWindow : Window
    {
        public ManagementWindow(INavigationStore navigationStore)
        {
            InitializeComponent();
            DataArea.Children.Add(navigationStore.GetUserControl<FolderManagementUC>());
        }
    }
}