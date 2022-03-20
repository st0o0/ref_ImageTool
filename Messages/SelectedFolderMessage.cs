using ImageTool.DBModels;

namespace ImageTool.Messages
{
    public class SelectedFolderMessage
    {
        private readonly Folder folder;

        public SelectedFolderMessage(Folder folder)
        {
            this.folder = folder;
        }

        public Folder Folder
        {
            get => folder;
        }
    }
}