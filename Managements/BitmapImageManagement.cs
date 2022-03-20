using ImageTool.Managements.Interfaces;
using JSLibrary.TPL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = ImageTool.DBModels.Image;

namespace ImageTool.Managements
{
    public class BitmapImageManagement : IBitmapImageManagement
    {
        public async Task ClearBitmapImagesAsync(IEnumerable<BitmapImage> images, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                images.ToList().Clear();
                GC.Collect();
            }, cancellationToken);
        }

        public async Task<IEnumerable<BitmapImage>> LoadBitmapImagesAsync(IEnumerable<Image> images, Dispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            return await ParallelTask.TaskManyAsync(images, x => GetBitmapImageFromImageAsync(x, dispatcher, cancellationToken), cancellationToken);
        }

        private async Task<BitmapImage> GetBitmapImageFromImageAsync(Image image, Dispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            byte[] downloadresult = await DownloadImageAsync(image?.LargePath, cancellationToken);
            return await dispatcher.InvokeAsync(new Func<BitmapImage>(() =>
            {
                var bitmapimage = new BitmapImage();
                using (var ms = new MemoryStream(downloadresult))
                {
                    bitmapimage.BeginInit();
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.StreamSource = ms;
                    bitmapimage.UriSource = new Uri(image?.LargePath);
                    bitmapimage.EndInit();
                    bitmapimage.Freeze();
                }
                return bitmapimage;
            }), DispatcherPriority.Send);
        }

        private async Task<byte[]> DownloadImageAsync(string url, CancellationToken cancellationToken = default)
        {
            return await new HttpClient().GetByteArrayAsync(new Uri(@url), cancellationToken);
        }
    }
}