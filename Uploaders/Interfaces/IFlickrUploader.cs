using FlickrNet;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.Uploaders.Interfaces
{
    public interface IFlickrUploader
    {
        void OnUploadProgress(EventHandler<UploadProgressEventArgs> handler);

        Task<string> UploadAsync(Stream stream, string fileName, string title, string description, bool isPublic, CancellationToken cancellationToken = default);
    }
}