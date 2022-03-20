using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class FolderImageLink : IDBModel
    {
        public int Id { get; set; }
        public int FolderId { get; set; }
        public int ImageId { get; set; }

        public virtual Folder Folder { get; set; }
        public virtual Image Image { get; set; }
    }
}