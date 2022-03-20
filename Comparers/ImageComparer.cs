using ImageTool.DBModels;
using System.Collections.Generic;

namespace ImageTool.Comparers
{
    public class ImageComparer : IComparer<Image>
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public int Compare(Image? x, Image? y)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            if (x is null || y is null)
            {
                return 0;
            }

            if (x?.Id > y?.Id)
            {
                return 1;
            }
            else if (x?.Id < y?.Id)
            {
                return -1;
            }
            return 0;
        }
    }
}