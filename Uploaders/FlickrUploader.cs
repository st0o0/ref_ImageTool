using FlickrNet;
using ImageTool.Helpers.Interfaces;
using ImageTool.Uploaders.Interfaces;
using InputBoxLibrary.WPF.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ImageTool.Uploaders
{
    public class FlickrUploader : IFlickrUploader
    {
        private readonly Flickr flickr;

        public FlickrUploader(IFlickrAuthHelper flickrAuthHelper)
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

        public void OnUploadProgress(EventHandler<UploadProgressEventArgs> handler)
        {
            flickr.OnUploadProgress += handler;
        }

        public Task<string> UploadAsync(Stream stream, string fileName, string title, string description, bool isPublic, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                return flickr.UploadPicture(stream, fileName, title, description, "", isPublic, true, true, ContentType.Photo, SafetyLevel.Safe, HiddenFromSearch.Hidden);
            }, cancellationToken);
        }
    }
}