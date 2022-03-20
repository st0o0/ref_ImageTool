using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Managements.Interfaces;
using ImageTool.Objects;
using ImageTool.Uploaders.Interfaces;
using JSLibrary.Logics.Business.Interfaces;
using JSLibrary.TPL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.ViewModels
{
    public abstract class EditableViewModelBase : ViewableViewModelBase
    {
        private readonly IServiceProvider serviceProvider;

        protected EditableViewModelBase(
            IServiceProvider serviceProvider,
            IFlickrManagement flickrManagement,
            IFileManagement fileManagement,
            IExifManagement exifManagement,
            IBitmapImageManagement bitmapImageManagement,
            IBusinessLogicBase<Folder, DBContext> folderBL,
            IBusinessLogicBase<Image, DBContext> imageBL,
            IBusinessLogicBase<Exif, DBContext> exifBL,
            IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL) : base(bitmapImageManagement, folderBL, imageBL, exifBL, folderImageLinkBL)
        {
            this.serviceProvider = serviceProvider;

            this.FlickrManagement = flickrManagement;
            this.FileManagement = fileManagement;
            this.ExifManagement = exifManagement;
        }

        protected IFlickrManagement FlickrManagement { get; init; }

        protected IFileManagement FileManagement { get; init; }

        protected IExifManagement ExifManagement { get; init; }

        protected bool ExistsFolder(IDictionary<string, string> dicionary, List<Folder> folders)
        {
            return folders.Exists(x => (x.Date == dicionary["Date"] && x.Location == dicionary["Location"]));
        }

        protected async Task<ObservableCollection<ImageFile>> GetImageFilesAsync(string folderpath, CancellationToken cancellationToken = default)
        {
            return (ObservableCollection<ImageFile>)await ParallelTask.TaskManyAsync(this.FileManagement.GetAllFileNamesFromFolder(folderpath), x =>
            {
                string fullrawfilepath = FileManagement.GetFullRAWFilePath(x, folderpath);
                string fullpngfilepath = FileManagement.GetFullPNGFilePath(x, folderpath);
                string rawfilepath = FileManagement.GetRAWFilePath(fullrawfilepath);
                string pngfilepath = FileManagement.GetPNGFilePath(fullpngfilepath);
                return new ImageFile(x, rawfilepath, pngfilepath, fullrawfilepath, fullpngfilepath);
            }, cancellationToken);
        }

        protected async Task<IEnumerable<int>> AddImagesWithExifsAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default)
        {
            List<int> result = new();
            foreach (Image image in images)
            {
                await ExifBL.AddAsync(image.Exif, cancellationToken);
                image.ExifId = image.Exif.Id;
                await ImageBL.AddAsync(image, cancellationToken);
                result.Add(image.Id);
            }
            return result;
        }

        protected async Task AddPhotosToPhotoSetAsync(IEnumerable<string> photoIds, string photoSetId, CancellationToken cancellationToken = default)
        {
            await ParallelTask.TaskManyAsync(photoIds, async x => await FlickrManagement.AddPhotoToPhotoSetAsync(photoSetId, x), cancellationToken);
        }

        protected async Task UpdateFolderWithPhotoSetAsync(Folder folder, CancellationToken cancellationToken = default)
        {
            await FolderBL.UpdateAsync(folder, cancellationToken);
            await FlickrManagement.PhotoSetEditMetaAsync(folder.PhotoSetId, $"{folder.Location}_{folder.Date}", folder.Description ?? "");
        }

        protected async Task UpdateImagesWithPhotos(IEnumerable<Image> images, CancellationToken cancellationToken = default)
        {
            foreach (Image image in images)
            {
                await ImageBL.UpdateAsync(image, cancellationToken);
                await FlickrManagement.PhotoEditMetaAsync(image.PhotoId, image.Name, image.Description);
            }
        }

        protected async Task DeletePhotoSet(string photosetId)
        {
            await FlickrManagement.DeletePhotoSetAsync(photosetId);
        }

        protected async Task DeletePhotos(List<string> photoIds)
        {
            await ParallelTask.TaskManyAsync(photoIds, DeletePhoto);
        }

        protected async Task DeletePhoto(string photoId)
        {
            await FlickrManagement.DeletePhotoAsync(photoId);
        }

        protected async Task<IEnumerable<FolderImageLink>> CreateFolderImageLinks(int folderId, IEnumerable<int> imageIds, CancellationToken cancellationToken = default)
        {
            return await ParallelTask.TaskManyAsync(imageIds, x =>
            {
                return new FolderImageLink
                {
                    FolderId = folderId,
                    ImageId = x,
                };
            }, cancellationToken);
        }

        protected async Task<List<Image>> CreateImagesWithUpload(IEnumerable<ImageFile> imageFiles, CancellationToken cancellationToken = default)
        {
            FlickrManagement.TestLogin();
            List<Image> result = new();
            foreach (ImageFile imageFile in imageFiles)
            {
                result.Add(await CreateImageWithUpload(imageFile, cancellationToken));
            }

            return result;
        }

        protected async Task<Image> CreateImageWithUpload(ImageFile imageFile, CancellationToken cancellationToken = default)
        {
            IFlickrUploader flickrUploader = this.serviceProvider.GetRequiredService<IFlickrUploader>();
            flickrUploader.OnUploadProgress((s, e) =>
            {
                imageFile.UploadProgress = (int)((e.BytesSent * 100) / e.TotalBytesToSend);
            });
            Stream stream = new FileStream(path: imageFile.FullPngFilePath, mode: FileMode.Open);
            string photoId = await flickrUploader.UploadAsync(stream, imageFile.FileName, imageFile.FileName, "", true);
            Task<Exif> exif = ExifManagement.GetExifFromFilePathAsync(imageFile.FullRawFilePath, cancellationToken);
            Task<string> orginalPath = FlickrManagement.GetOriginalPathAsync(photoId, cancellationToken);
            Task<string> largePath = FlickrManagement.GetLargePathAsync(photoId, cancellationToken);
            Task<string> thumbnailPath = FlickrManagement.GetThumbnailPathAsync(photoId, cancellationToken);
            Image image = new()
            {
                PhotoId = photoId,
                Exif = await exif,
                OriginalPath = await orginalPath,
                LargePath = await largePath,
                ThumbnailPath = await thumbnailPath,
                Name = imageFile.FileName,
            };
            imageFile.UploadComplete = true;
            await stream.FlushAsync(cancellationToken);

            return image;
        }

        protected async Task<string> CreatePhotoSetIdWithPrimaryPhotoId(string title, string primaryPhotoId)
        {
            return await FlickrManagement.CreatePhotoSetAsync(title, primaryPhotoId);
        }
    }
}