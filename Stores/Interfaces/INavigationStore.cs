using System.Windows;
using System.Windows.Controls;

namespace ImageTool.Stores.Interfaces
{
    public interface INavigationStore
    {
        UserControl GetUserControl<T>() where T : UserControl;

        Window GetWindow<T>() where T : Window;
    }
}