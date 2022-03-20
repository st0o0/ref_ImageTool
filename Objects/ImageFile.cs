using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageTool.Objects
{
    public class ImageFile : INotifyPropertyChanged
    {
        private int uploadProgress;
        private bool uploadComplete;

        public ImageFile(string filename, string rawfilepath, string pngfilepath, string fullrawfilepath, string fullpngfilepath)
        {
            FileName = filename;
            RawFilePath = rawfilepath;
            PngFilePath = pngfilepath;
            FullRawFilePath = fullrawfilepath;
            FullPngFilePath = fullpngfilepath;
        }

        public string FileName { get; private set; }
        public string FullPngFilePath { get; private set; }
        public string FullRawFilePath { get; private set; }
        public string PngFilePath { get; private set; }
        public string RawFilePath { get; private set; }

        public int UploadProgress
        {
            get => uploadProgress;
            set
            {
                uploadProgress = value;
                NotifyPropertyChanged();
            }
        }

        public bool UploadComplete
        {
            get => uploadComplete;
            set
            {
                uploadComplete = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}