using ImageTool.ViewModels.ManagementVM;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageTool.Views.Management
{
    /// <summary>
    /// Interaction logic for FolderManagementUC.xaml
    /// </summary>
    public partial class FolderManagementUC : UserControl
    {
        public FolderManagementUC(FolderManagementVM folderManagementVM)
        {
            InitializeComponent();
            this.DataContext = folderManagementVM;
            Loaded += (s, e) =>
            {
                Window window = Window.GetWindow(this);
                window.Closing += folderManagementVM.OnWindowClosing;
                folderManagementVM.DialogResultAction = (t) => window.DialogResult = t;
            };
            zoomBorder.MouseRightButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                if (e.RightButton == Mouse.RightButton)
                {
                    zoomBorder.Reset();
                }
            };
        }
    }
}