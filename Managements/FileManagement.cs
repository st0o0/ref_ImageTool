using ImageTool.Managements.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageTool.Managements
{
    public class FileManagement : IFileManagement
    {
        private readonly ILogger<FileManagement> logger;

        private const string rawExtensions = ".arw";
        private const string pngExtensions = ".png";

        public FileManagement(ILogger<FileManagement> logger)
        {
            this.logger = logger;
        }

        public IDictionary<string, string> GetFoldernameSplited(string folderpath)
        {
            string foldername = GetFoldername(folderpath);
            string date = GetDate(foldername);
            string location = GetLocation(foldername);
            var result = new Dictionary<string, string>
            {
                { "Location", location },
                { "Date", date }
            };
            return result;
        }

        public IEnumerable<string> GetAllFileNamesFromFolder(string folder)
        {
            string path = GetPngFolderpath(folder);
            var result = Directory.GetFiles(path, pngExtensions).Select(x => RemoveExtension(GetOnlyFileName(x))).ToList();
            return result.ToArray();
        }

        public string GetFileFolderPath(string path)
        {
            return Regex.Match(path, @"([\\]{1,2}RAW[\\]{1,2}[A-Za-z]*[0-9]*[.]arw$)").Value;
        }

        public string GetFullPNGFilePath(string filenameWithoutExtension, string folder)
        {
            return Path.Combine(folder, "PNG", ChangeExtensionToPNG(filenameWithoutExtension));
        }

        public string GetFullRAWFilePath(string filenameWithoutExtension, string folder)
        {
            return Path.Combine(GetRawFolderpath(folder), ChangeExtensionToARW(filenameWithoutExtension));
        }

        public string GetPNGFilePath(string fullpngfilepath)
        {
            return Regex.Match(fullpngfilepath, @"([\\]{1,2}PNG[\\]{1,2}[A-Za-z]*[0-9]*[.]png$)").Value;
        }

        public string GetRAWFilePath(string fullrawfilepath)
        {
            return Regex.Match(fullrawfilepath, @"([\\]{1,2}RAW[\\]{1,2}[A-Za-z]*[0-9]*[.]arw$)").Value;
        }

        private string ChangeExtensionToPNG(string filenameWithoutExtension)
        {
            return Path.ChangeExtension(filenameWithoutExtension, pngExtensions);
        }

        private string ChangeExtensionToARW(string filenameWithoutExtension)
        {
            return Path.ChangeExtension(filenameWithoutExtension, rawExtensions);
        }

        private string GetPngFolderpath(string folderpath)
        {
            return Path.Combine(folderpath, "PNG");
        }

        private string GetRawFolderpath(string folderpath)
        {
            return Path.Combine(folderpath, "RAW");
        }

        private string GetOnlyFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }

        private string RemoveExtension(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        private string GetFoldername(string folderpath)
        {
            return Regex.Match(folderpath, "([a-zA-Z_0-9]*[0-9]*$)").Value;
        }

        private string GetDate(string foldername)
        {
            return Regex.Match(foldername, "([0-9]+$)").Value;
        }

        private string GetLocation(string foldername)
        {
            return Regex.Match(foldername, "(^[a-zA-Z]*[_][a-zA-z]*[^_0-9])|(^[a-zA-Z]*[^_]*)").Value;
        }
    }
}