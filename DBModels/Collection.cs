using JSLibrary.Logics.Business.Interfaces;
using System.Collections.Generic;

namespace ImageTool.DBModels
{
    public partial class Collection : IDBModel
    {
        public Collection()
        {
            CollectionImageLinks = new HashSet<CollectionImageLink>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CollectionImageLink> CollectionImageLinks { get; set; }
    }
}