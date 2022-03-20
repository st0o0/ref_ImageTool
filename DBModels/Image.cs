using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class Image : IDBModel
    {
        public Image()
        {
            CollectionImageLinks = new HashSet<CollectionImageLink>();
            FolderImageLinks = new HashSet<FolderImageLink>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoId { get; set; }
        public string ThumbnailPath { get; set; }
        public string OriginalPath { get; set; }
        public string LargePath { get; set; }
        public int ExifId { get; set; }

        public virtual Exif Exif { get; set; }
        public virtual ICollection<CollectionImageLink> CollectionImageLinks { get; set; }
        public virtual ICollection<FolderImageLink> FolderImageLinks { get; set; }
    }
}