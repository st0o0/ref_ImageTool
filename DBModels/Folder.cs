using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class Folder : IDBModel
    {
        public Folder()
        {
            FolderImageLinks = new HashSet<FolderImageLink>();
        }

        public int Id { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string PhotoSetId { get; set; }

        public virtual ICollection<FolderImageLink> FolderImageLinks { get; set; }
    }
}