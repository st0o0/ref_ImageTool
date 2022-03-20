using FlickrNet;
using ImageTool.Helpers.Interfaces;
using ImageTool.Settings.Interfaces;

namespace ImageTool.Helpers
{
    public class FlickrAuthHelper : IFlickrAuthHelper
    {
        private readonly string apiKey;
        private readonly string sharedKey;

        private static Flickr flickr;
        private static Flickr authflickr;

        public FlickrAuthHelper(IFlickrSettings settings)
        {
            this.apiKey = settings.ApiKey;
            this.sharedKey = settings.SharedKey;
        }

        public Flickr GetInstance()
        {
            return flickr ??= new Flickr(apiKey, sharedKey);
        }

        public Flickr GetAuthInstance()
        {
            return authflickr ??= new Flickr(apiKey, sharedKey)
            {
                OAuthAccessToken = OAuthToken?.Token,
                OAuthAccessTokenSecret = OAuthToken?.TokenSecret
            };
        }

        public OAuthAccessToken OAuthToken
        {
            get
            {
                return Properties.Settings.Default.OAuthToken;
            }
            set
            {
                Properties.Settings.Default.OAuthToken = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}