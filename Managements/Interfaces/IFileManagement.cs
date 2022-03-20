using System.Collections.Generic;

namespace ImageTool.Managements.Interfaces
{
    public interface IFileManagement
    {
        public IDictionary<string, string> GetFoldernameSplited(string folderPath);

        public IEnumerable<string> GetAllFileNamesFromFolder(string folderPath);

        public string GetFileFolderPath(string path);

        public string GetFullPNGFilePath(string filenameWithoutExtension, string folderPath);

        public string GetFullRAWFilePath(string filenameWithoutExtension, string folderPath);

        public string GetPNGFilePath(string fullpngfilepath);

        public string GetRAWFilePath(string fullrawfilepath);
    }
}