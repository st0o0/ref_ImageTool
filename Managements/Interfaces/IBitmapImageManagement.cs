using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = ImageTool.DBModels.Image;

namespace ImageTool.Managements.Interfaces
{
    public interface IBitmapImageManagement
    {
        Task ClearBitmapImagesAsync(IEnumerable<BitmapImage> images, CancellationToken cancellationToken = default);

        Task<IEnumerable<BitmapImage>> LoadBitmapImagesAsync(IEnumerable<Image> images, Dispatcher dispatcher, CancellationToken cancellationToken = default);
    }
}