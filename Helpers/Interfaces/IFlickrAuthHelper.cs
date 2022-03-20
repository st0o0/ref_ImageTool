using FlickrNet;

namespace ImageTool.Helpers.Interfaces
{
    public interface IFlickrAuthHelper
    {
        Flickr GetInstance();

        Flickr GetAuthInstance();

        OAuthAccessToken OAuthToken { get; set; }
    }
}