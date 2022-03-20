using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class CollectionImageLink : IDBModel
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int ImageId { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual Image Image { get; set; }
    }
}