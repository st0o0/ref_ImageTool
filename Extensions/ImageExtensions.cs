using ImageTool.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageTool.Extensions
{
    public static class ImageExtensions
    {
        public static BitmapImage GetBitmapImage(this Image image, IEnumerable<BitmapImage> bitmaps, Action good, Action fail)
        {
            BitmapImage result = bitmaps?.FirstOrDefault(x => x.UriSource.AbsoluteUri == image.LargePath);
            if (result == null)
            {
                fail?.Invoke();
            }
            else
            {
                good?.Invoke();
            }
            return result;
        }

        public static (IEnumerable<string>, string) GetPhotoSetItems(this IEnumerable<Image> images)
        {
            int index = 0;
            if (images.Count() > 1)
            {
                index = new Random().Next(0, images.Count() - 1);
            }
            string primaryPhotoId = images.ToList()[index].PhotoId;
            IEnumerable<string> photoIds = images.Where(x => x.PhotoId != primaryPhotoId).Select(x => x.PhotoId);
            return (photoIds, primaryPhotoId);
        }
    }
}