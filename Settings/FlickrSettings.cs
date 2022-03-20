using ImageTool.Settings.Interfaces;

namespace ImageTool.Settings
{
    public class FlickrSettings : IFlickrSettings
    {
        public string ApiKey { get; init; }
        public string SharedKey { get; init; }
    }
}