using Ookii.Dialogs.Wpf;
using System.Threading.Tasks;

namespace ImageTool.Dialogs
{
    public static class OpenFolder
    {
        public static async Task<string> DialogAsync()
        {
            return await Task.Run(() =>
            {
                return Dialog();
            });
        }

        public static string Dialog()
        {
            string result = null;
            VistaFolderBrowserDialog dialog = new();
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == true)
            {
                result = dialog.SelectedPath;
            }
            return result;
        }
    }
}