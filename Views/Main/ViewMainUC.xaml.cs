using ImageTool.ViewModels.MainVM;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageTool.Views.Main
{
    /// <summary>
    /// Interaction logic for ViewMainUC.xaml
    /// </summary>
    public partial class ViewMainUC : UserControl
    {
        public ViewMainUC(ViewMainVM viewMainVM)
        {
            InitializeComponent();
            this.DataContext = viewMainVM;
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