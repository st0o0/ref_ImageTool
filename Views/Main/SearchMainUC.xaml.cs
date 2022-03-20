using ImageTool.ViewModels.MainVM;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageTool.Views.Main
{
    /// <summary>
    /// Interaction logic for SearchUC.xaml
    /// </summary>
    public partial class SearchMainUC : UserControl
    {
        public SearchMainUC(SearchMainVM searchMainVM)
        {
            InitializeComponent();
            this.DataContext = searchMainVM;
            this.Loaded += (s, e) =>
            {
                Window parent = Window.GetWindow(this);
                if (searchMainVM.CloseAction == null)
                {
                    searchMainVM.CloseAction = new Action(() => parent.Close());
                }
                parent.Closing += searchMainVM.OnWindowClosing;
            };
        }
    }
}