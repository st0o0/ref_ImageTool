using ImageTool.Comparers;
using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Extensions;
using ImageTool.Managements.Interfaces;
using ImageTool.Messages;
using ImageTool.Struct;
using JSLibrary.Logics.Business.Interfaces;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageTool.ViewModels.MainVM
{
    public class ViewMainVM : ViewableViewModelBase
    {
        private Folder selectedFolder;
        private Image selectedImage;
        private BitmapImage selectedBitmapImage;

        private List<BitmapImage> bitmapImages;
        private List<Image> images;

        private ObservableCollection<Exifs> exif = new();

        private bool imageDownloadFailedVisible;
        private bool imageVisible;
        private bool imageListViewEnable;

        public ViewMainVM(
            IBitmapImageManagement bitmapImageManagement,
            IBusinessLogicBase<Folder, DBContext> folderBL,
            IBusinessLogicBase<Image, DBContext> imageBL,
            IBusinessLogicBase<Exif, DBContext> exifBL,
            IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL
            ) : base(bitmapImageManagement, folderBL, imageBL, exifBL, folderImageLinkBL)
        {
            Messenger.Default.Register<SelectedFolderMessage>(this, "MainView", async (SFM) =>
            {
                if (selectedFolder?.Id != SFM.Folder.Id)
                {
                    bitmapImages?.Clear();
                    SelectedBitmapImage = null;
                    await LoadCompleteFolder(SFM.Folder, ct);
                }
            });

            Messenger.Default.Register<RefreshCommandMessage>(this, "MainView", async (RCM) =>
            {
                if (RCM.Refresh)
                {
                    var selectedImage = SelectedImage;
                    var folder = selectedFolder;
                    await LoadCompleteFolder(folder, ct);
                    UpdateUI(() =>
                    {
                        SelectedImage = Images.SingleOrDefault(x => x.Id == selectedImage?.Id);
                    });
                }
            });
        }

        #region properties

        public BitmapImage SelectedBitmapImage
        {
            get => selectedBitmapImage;
            set
            {
                selectedBitmapImage = value;
                RaisePropertyChanged(nameof(SelectedBitmapImage));
            }
        }

        public Image SelectedImage
        {
            get => selectedImage;
            set
            {
                if (value != null)
                {
                    selectedImage = value;
                    SelectedBitmapImage = SelectedImage.GetBitmapImage(bitmapImages, GoodBitmapImage, FailBitmapImage);
                    SetExif(SelectedImage, Exif);
                    RaisePropertyChanged(nameof(ImageDescription));
                    RaisePropertyChanged(nameof(SelectedImage));
                }
            }
        }

        public ObservableCollection<Exifs> Exif
        {
            get => exif;
            set
            {
                exif = value;
                RaisePropertyChanged(nameof(exif));
            }
        }

        public List<Image> Images
        {
            get => images;
            set
            {
                images = value;
                RaisePropertyChanged(nameof(Images));
            }
        }

        public string FolderLocation
        {
            get => selectedFolder?.Location;
        }

        public string FolderDate
        {
            get => selectedFolder?.Date;
        }

        public string FolderDescription
        {
            get => selectedFolder?.Description;
        }

        public string ImageDescription
        {
            get => selectedImage?.Description;
        }

        public bool ImageDownloadFailedVisible
        {
            get => imageDownloadFailedVisible;
            set
            {
                imageDownloadFailedVisible = value;
                RaisePropertyChanged(nameof(ImageDownloadFailedVisible));
            }
        }

        public bool ImageVisible
        {
            get => imageVisible;
            set
            {
                imageVisible = value;
                RaisePropertyChanged(nameof(ImageVisible));
            }
        }

        public bool ImageListViewEnable
        {
            get => imageListViewEnable;
            set
            {
                imageListViewEnable = value;
                RaisePropertyChanged(nameof(ImageListViewEnable));
            }
        }

        #endregion properties

        #region methode

        public override void Dispose()
        {
            Messenger.Default.Unregister(this);
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private void GoodBitmapImage()
        {
            ImageDownloadFailedVisible = false;
            ImageVisible = true;
        }

        private void FailBitmapImage()
        {
            ImageVisible = false;
            ImageDownloadFailedVisible = true;
        }

        private async Task LoadCompleteFolder(Folder folder, CancellationToken ct)
        {
            int folderId = folder.Id;
            await FolderBL.GetAsync(folderId, ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => selectedFolder = t.Result), ct);
            await LoadImages(folderId, ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Images = t.Result.ToList()), ct);
            Images?.Sort(new ImageComparer());
            await RefreshWithoutBitmapImage();
            await LoadBitmapImages(Images, ct, () =>
            {
                ImageListViewEnable = false;
                ImageDownloadFailedVisible = false;
                ImageVisible = false;
            }, () =>
            {
                ImageListViewEnable = true;
                ImageVisible = true;
            }).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => bitmapImages = t.Result.ToList()), ct);
            await Task.Delay(250, ct);
            UpdateUI(() =>
            {
                SelectedImage = Images?[0];
            });
            await Refresh();
        }

        private async Task RefreshWithoutBitmapImage()
        {
            await UpdateUIAsync(() =>
            {
                RaisePropertyChanged(nameof(Exif));
                RaisePropertyChanged(nameof(Images));
                RaisePropertyChanged(nameof(FolderDescription));
                RaisePropertyChanged(nameof(FolderDate));
                RaisePropertyChanged(nameof(FolderLocation));
                RaisePropertyChanged(nameof(ImageDescription));
            });
        }

        private async Task Refresh()
        {
            await UpdateUIAsync(() =>
            {
                RaisePropertyChanged(nameof(Exif));
                RaisePropertyChanged(nameof(Images));
                RaisePropertyChanged(nameof(FolderDescription));
                RaisePropertyChanged(nameof(FolderDate));
                RaisePropertyChanged(nameof(FolderLocation));
                RaisePropertyChanged(nameof(ImageDescription));
                RaisePropertyChanged(nameof(SelectedBitmapImage));
            });
        }

        #endregion methode
    }
}