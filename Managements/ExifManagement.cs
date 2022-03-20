using ImageTool.DBModels;
using ImageTool.Managements.Interfaces;
using MetadataExtractor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.Managements
{
    public class ExifManagement : IExifManagement
    {
        public async Task<Exif> GetExifFromFilePathAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var directories = ImageMetadataReader.ReadMetadata(fileName);
            return await GetExifAsync(directories, cancellationToken);
        }

        private async Task<Exif> GetExifAsync(IReadOnlyList<Directory> directories, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var tagIds = new int[] { 271, 272, 306, 33434, 33437, 34855, 37386, 42036 };
                Dictionary<int, string> valuePairs = new();
                foreach (int i in tagIds)
                {
                    valuePairs.Add(i, directories.Where(x => x.ContainsTag(i)).FirstOrDefault()?.GetDescription(i));
                }

                return new Exif()
                {
                    Manufacturer = valuePairs[271],
                    Model = valuePairs[272],
                    DateTime = valuePairs[306],
                    ExposureTime = ValidationExposureTime(valuePairs[33434]),
                    Aperture = valuePairs[33437],
                    Iso = int.Parse(valuePairs[34855]),
                    FocalLength = valuePairs[37386],
                    LensInfo = Regex.Match(valuePairs[42036], @".*[A-Za-z]").Value,
                };
            }, cancellationToken);
        }

        private string ValidationExposureTime(string exposureTime)
        {
            var match = Regex.Match(exposureTime, @"1[\\/][0-9]{0,4}");
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return exposureTime;
            }
        }
    }
}