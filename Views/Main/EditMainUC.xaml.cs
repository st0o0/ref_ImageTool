using ImageTool.ViewModels.MainVM;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageTool.Views.Main
{
    /// <summary>
    /// Interaction logic for EditMainUC.xaml
    /// </summary>
    public partial class EditMainUC : UserControl
    {
        public EditMainUC(EditMainVM editMainVM)
        {
            InitializeComponent();
            this.DataContext = editMainVM;
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