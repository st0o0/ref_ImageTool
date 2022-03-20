using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.Managements.Interfaces
{
    public interface IFlickrManagement
    {
        Task<string> CreatePhotoSetAsync(string title, string primaryPhotoId, CancellationToken cancellationToken = default);

        //Task<string> CreateGallerieAsync(string title, string primaryPhotoId);

        Task AddPhotoToPhotoSetAsync(string photoSetId, string photoId, CancellationToken cancellationToken = default);

        //Task AddPhotoToGalleireAsync(string gallerieId, string photoId);

        Task<string> GetOriginalPathAsync(string photoId, CancellationToken cancellationToken = default);

        Task<string> GetLargePathAsync(string photoId, CancellationToken cancellationToken = default);

        Task<string> GetThumbnailPathAsync(string photoId, CancellationToken cancellationToken = default);

        Task DeletePhotoSetAsync(string photoSetId, CancellationToken cancellationToken = default);

        Task DeletePhotoAsync(string photoId, CancellationToken cancellationToken = default);

        Task PhotoSetEditMetaAsync(string photoSetId, string title, string description, CancellationToken cancellationToken = default);

        Task PhotoEditMetaAsync(string photoId, string title, string description, CancellationToken cancellationToken = default);

        string TestLogin();
    }
}