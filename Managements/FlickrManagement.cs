using FlickrNet;
using ImageTool.Helpers.Interfaces;
using ImageTool.Managements.Interfaces;
using InputBoxLibrary.WPF.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ImageTool.Managements
{
    public class FlickrManagement : IFlickrManagement
    {
        private readonly Flickr flickr;

        public FlickrManagement(IFlickrAuthHelper flickrAuthHelper)
        {
            if (flickrAuthHelper.OAuthToken == null)
            {
                flickr = flickrAuthHelper.GetInstance();
                var requestToken = flickr.OAuthGetRequestToken("oob");
                var url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Delete);
                (var t, string verifier) = InputBox.Show("Enters the Verfier", "Flickr Login", url, MessageBoxButton.OKCancel);
                if (t == MessageBoxResult.OK)
                {
                    var accessToken = flickr.OAuthGetAccessToken(requestToken, verifier);
                    flickrAuthHelper.OAuthToken = accessToken;
                }
            }
            flickr = flickrAuthHelper.GetAuthInstance();
        }

        public async Task<string> CreatePhotoSetAsync(string title, string primaryPhotoId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return flickr.PhotosetsCreate(title, primaryPhotoId).PhotosetId;
            }, cancellationToken);
        }

        public async Task AddPhotoToPhotoSetAsync(string photosetId, string photoId, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                flickr.PhotosetsAddPhoto(photosetId, photoId);
            }, cancellationToken);
        }

        public async Task<string> GetOriginalPathAsync(string photoId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return flickr.PhotosGetInfo(photoId).OriginalUrl;
            }, cancellationToken);
        }

        public async Task<string> GetLargePathAsync(string photoId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return flickr.PhotosGetInfo(photoId).LargeUrl;
            }, cancellationToken);
        }

        public async Task<string> GetThumbnailPathAsync(string photoId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return flickr.PhotosGetInfo(photoId).ThumbnailUrl;
            }, cancellationToken);
        }

        public async Task DeletePhotoSetAsync(string photoSetId, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                flickr.PhotosetsDelete(photoSetId);
            }, cancellationToken);
        }

        public async Task DeletePhotoAsync(string photoId, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                flickr.PhotosDelete(photoId);
            }, cancellationToken);
        }

        public async Task PhotoSetEditMetaAsync(string photosetId, string title, string description, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                flickr.PhotosetsEditMeta(photosetId, title, description);
            }, cancellationToken);
        }

        public async Task PhotoEditMetaAsync(string photoId, string title, string description, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                flickr.PhotosSetMeta(photoId, title, description);
            }, cancellationToken);
        }

        public string TestLogin()
        {
            var t = flickr.TestLogin();
            return t.UserName;
        }
    }
}