using System;
using System.Windows;

namespace ImageTool.MessageBoxes
{
    public static class CustomMessageBox
    {
        public static void SaveMessageBox(Action yesAction, Action noAction)
        {
            var result = MessageBox.Show("Speichern?", "Save", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                yesAction?.Invoke();
            }
            else if (result == MessageBoxResult.No)
            {
                noAction?.Invoke();
            }
        }

        public static void DeleteMessageBox(Action yesAction, Action noAction)
        {
            var result = MessageBox.Show("Löschen?", "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                yesAction?.Invoke();
            }
            else if (result == MessageBoxResult.No)
            {
                noAction?.Invoke();
            }
        }

        public static void UploadMessagebox(Action yesAction)
        {
            var result = MessageBox.Show("Die Bilder werden auf Flickr hochgeladen", "Upload", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                yesAction?.Invoke();
            }
            else
            {
                InformationMessageBox("Es werden keine Bilder hochgeladen");
            }
        }

        public static void InformationMessageBox(string message)
        {
            var result = MessageBox.Show(message, "InformationMessageBox", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
            }
        }

        public static void RepeatMessageBox(Action yesAction, Action noAction)
        {
            var result = MessageBox.Show("Wiederholen?", "Repeat", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                yesAction?.Invoke();
            }
            else if (result == MessageBoxResult.No)
            {
                noAction?.Invoke();
            }
        }

        public static void ErrorRepeatMessageBox(string message, Action yesAction, Action noAction)
        {
            var result = MessageBox.Show(message + "\nWiederholen?", "ErrorRepeat", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                yesAction?.Invoke();
            }
            else if (result == MessageBoxResult.No)
            {
                noAction?.Invoke();
            }
        }

        public static void ErrorMessageBox(Exception e)
        {
            var result = MessageBox.Show(e.Message, "ErrorMessageBox", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
            }
        }
    }
}