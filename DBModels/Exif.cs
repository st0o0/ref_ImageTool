using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class Exif : IDBModel
    {
        public Exif()
        {
            Images = new HashSet<Image>();
        }

        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string DateTime { get; set; }
        public string ExposureTime { get; set; }
        public string Aperture { get; set; }
        public int? Iso { get; set; }
        public string FocalLength { get; set; }
        public string LensInfo { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}