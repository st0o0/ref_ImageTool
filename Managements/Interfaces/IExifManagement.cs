using ImageTool.DBModels;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.Managements.Interfaces
{
    public interface IExifManagement
    {
        Task<Exif> GetExifFromFilePathAsync(string fileName, CancellationToken cancellationToken = default);
    }
}