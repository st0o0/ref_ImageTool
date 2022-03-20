using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Managements.Interfaces;
using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageTool.ViewModels
{
    public abstract class ViewableViewModelBase : CustomViewModelBase
    {
        protected ViewableViewModelBase(
            IBitmapImageManagement bitmapImageManagement,
            IBusinessLogicBase<Folder, DBContext> folderBL,
            IBusinessLogicBase<Image, DBContext> imageBL,
            IBusinessLogicBase<Exif, DBContext> exifBL,
            IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL) : base(folderBL, imageBL, exifBL, folderImageLinkBL)
        {
            this.BitmapImageManagement = bitmapImageManagement;
        }

        protected IBitmapImageManagement BitmapImageManagement { get; init; }

        protected async Task<IEnumerable<Folder>> LoadFolders(CancellationToken cancellationToken = default)
        {
            return (await FolderBL.LoadAsync(cancellationToken)).ToList();
        }

        protected async Task<IEnumerable<Image>> LoadImages(int folderId, CancellationToken cancellationToken = default)
        {
            return (await FolderImageLinkBL.LoadAsync(cancellationToken)).Where(x => x.FolderId == folderId).Select(x => x.Image).ToList();
        }

        protected async Task<IEnumerable<BitmapImage>> LoadBitmapImages(List<Image> images, CancellationToken cancellationToken = default, Action beforeAction = null, Action afterAction = null)
        {
            beforeAction?.Invoke();
            IEnumerable<BitmapImage> result = await this.BitmapImageManagement.LoadBitmapImagesAsync(images, this.Dispatcher, cancellationToken);
            afterAction?.Invoke();
            return result;
        }

        private static IEnumerable<Folder> ApplyFilter(Func<Folder, bool> applyfunc, IEnumerable<Folder> folders)
        {
            return folders?.Where((item) => applyfunc(item)).ToList();
        }

        protected override void SetFilter(string searchtext, IEnumerable<Folder> folders, Action<IEnumerable<Folder>> filteredFolder)
        {
            if (!IsValid(searchtext))
            {
                filteredFolder(folders);
            }
            else
            {
                if (Regex.IsMatch(searchtext, "^[0-9]*$"))
                {
                    filteredFolder(ApplyFilter(new Func<Folder, bool>((item) => StartWithAsUpper(item.Date, searchtext)), folders));
                }
                else
                {
                    filteredFolder(ApplyFilter(new Func<Folder, bool>((item) => StartWithAsUpper(item.Location, searchtext)), folders));
                }
            }
        }
    }
}