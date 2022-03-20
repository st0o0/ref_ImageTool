using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Struct;
using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFLibrary.ViewModels;

namespace ImageTool.ViewModels
{
    public abstract class CustomViewModelBase : ViewModelBase
    {
        protected CancellationTokenSource cts = new();

        protected CancellationToken ct;

        protected CustomViewModelBase(IBusinessLogicBase<Folder, DBContext> folderBL, IBusinessLogicBase<Image, DBContext> imageBL, IBusinessLogicBase<Exif, DBContext> exifBL, IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL)
        {
            this.FolderBL = folderBL;
            this.ImageBL = imageBL;
            this.ExifBL = exifBL;
            this.FolderImageLinkBL = folderImageLinkBL;
        }

        public IBusinessLogicBase<Folder, DBContext> FolderBL { get; init; }

        public IBusinessLogicBase<Image, DBContext> ImageBL { get; init; }

        public IBusinessLogicBase<Exif, DBContext> ExifBL { get; init; }

        public IBusinessLogicBase<FolderImageLink, DBContext> FolderImageLinkBL { get; init; }

        protected void Cancel()
        {
            cts.Cancel();
        }

        protected bool IsValid(string val) => !string.IsNullOrEmpty(val);

        private string WithoutSpace(string val) => val.Replace(" ", "");

        protected bool ContainAsUpper(string var1, string var2) => WithoutSpace(var1.ToUpper()).Contains(WithoutSpace(var2.ToUpper()));

        protected bool StartWithAsUpper(string var1, string var2) => WithoutSpace(var1.ToUpper()).StartsWith(WithoutSpace(var2.ToUpper()));

        protected virtual void SetFilter(string searchtext, IEnumerable<Folder> folders, Action<IEnumerable<Folder>> filteredFolder)
        {
        }

        protected void SetExif(Image image, ObservableCollection<Exifs> exifs)
        {
            exifs.Clear();
            exifs.Add(new Exifs("Aperture", image?.Exif.Aperture));
            exifs.Add(new Exifs("ExposureTime", image?.Exif.ExposureTime));
            exifs.Add(new Exifs("ISO", image?.Exif.Iso.ToString()));
            exifs.Add(new Exifs("FocalLength", image?.Exif.FocalLength));
            exifs.Add(new Exifs("DateTime", image?.Exif.DateTime));
            exifs.Add(new Exifs("LensInfo", image?.Exif.LensInfo));
            exifs.Add(new Exifs("Model", image?.Exif.Model));
            exifs.Add(new Exifs("Manufacturer", image?.Exif.Manufacturer));
        }

        protected void IsTaskCompleteSuccessfully<T>(Task<T> t, Func<Task<T>, T> func)
        {
            if (t.IsCompletedSuccessfully)
            {
                func(t);
            }
            else if (t.IsFaulted)
            {
                MessageBox.Show(t.Exception.Message, "Error");
            }
        }

        protected async Task ProgressSimulation(Action<int> action, int delay = 250)
        {
            for (int i = 0; i < 101; i++)
            {
                await Task.Delay(delay);
                action(i);
            }
        }

        protected List<ListType> Except<ListType, HashType>(List<ListType> list1, List<ListType> list2, Func<ListType, HashType> func)
        {
            HashSet<HashType> list1Hash = new(list1.Select(x => func(x)));
            var result = list2.Where(m => !list1Hash.Contains(func(m)));
            return result.ToList();
        }
    }
}