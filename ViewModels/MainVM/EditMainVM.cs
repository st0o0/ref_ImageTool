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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFLibrary.RelayCommands;

namespace ImageTool.ViewModels.MainVM
{
    public class EditMainVM : EditableViewModelBase
    {
        private Folder selectedFolder;
        private BitmapImage selectedBitmapImage;
        private Image selectedImage;

        private ObservableCollection<Exifs> exif = new();
        private List<Image> images;

        private bool imageDownloadFailedVisible;

        private bool imageVisible;
        private bool imageListViewEnable;

        private bool imageInformationEditMode = true;
        private bool folderInformationEditMode = true;

        private List<BitmapImage> bitmapImages;

        private bool isSaved = true;

        public EditMainVM(
            IServiceProvider serviceProvider,
            IFlickrManagement flickrManagement,
            IFileManagement fileManagement,
            IExifManagement exifManagement,
            IBitmapImageManagement bitmapImageManagement,
            IBusinessLogicBase<Folder, DBContext> folderBL,
            IBusinessLogicBase<Image, DBContext> imageBL,
            IBusinessLogicBase<Exif, DBContext> exifBL,
            IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL) : base(serviceProvider, flickrManagement, fileManagement, exifManagement, bitmapImageManagement, folderBL, imageBL, exifBL, folderImageLinkBL)
        {
            Messenger.Default.Register<SelectedFolderMessage>(this, "MainEdit", async (SFM) =>
            {
                if (selectedFolder?.Id != SFM.Folder.Id)
                {
                    bitmapImages?.Clear();
                    SelectedBitmapImage = null;
                    await LoadCompleteFolder(SFM.Folder, ct);
                }
            });

            Messenger.Default.Register<RefreshCommandMessage>(this, "MainEdit", async (RCM) =>
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

            Messenger.Default.Register<SaveCommandMessage>(this, "MainEdit", async (SCM) =>
            {
                if (SCM.Save)
                {
                    await Save();
                }
                else
                {
                    HasSaved();
                }
            });
        }

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
                RaisePropertyChanged(nameof(Exif));
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

        public string FolderDescription
        {
            get => selectedFolder?.Description;
            set
            {
                if (value != null && selectedFolder != null)
                {
                    selectedFolder.Description = value;
                    HasChanged();
                    RaisePropertyChanged(nameof(FolderDescription));
                }
            }
        }

        public string FolderDate
        {
            get => selectedFolder?.Date;
            set
            {
                if (value != null && selectedFolder != null)
                {
                    selectedFolder.Date = value;
                    HasChanged();
                    RaisePropertyChanged(nameof(FolderDate));
                }
            }
        }

        public string FolderLocation
        {
            get => selectedFolder?.Location;
            set
            {
                if (value != null && selectedFolder != null)
                {
                    selectedFolder.Location = value;
                    HasChanged();
                    RaisePropertyChanged(nameof(FolderLocation));
                }
            }
        }

        public string ImageDescription
        {
            get => selectedImage?.Description;
            set
            {
                if (value != null && selectedImage != null)
                {
                    selectedImage.Description = value;
                    HasChanged();
                    RaisePropertyChanged(nameof(ImageDescription));
                }
            }
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

        public bool ImageInformationEditMode
        {
            get => imageInformationEditMode;
            set
            {
                imageInformationEditMode = value;
                RaisePropertyChanged(nameof(ImageInformationEditMode));
            }
        }

        public bool FolderInformationEditMode
        {
            get => folderInformationEditMode;
            set
            {
                folderInformationEditMode = value;
                RaisePropertyChanged(nameof(FolderInformationEditMode));
            }
        }

        public bool IsSaved
        {
            get => isSaved;
            set
            {
                isSaved = value;
                Task.Run(async () => await SendSaveMessage(isSaved));
                RaisePropertyChanged(nameof(IsSaved));
            }
        }

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

        private void HasChanged()
        {
            if (IsSaved)
            {
                IsSaved = !IsSaved;
            }
        }

        private void HasSaved()
        {
            if (!IsSaved)
            {
                IsSaved = !IsSaved;
            }
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
            }).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => bitmapImages = t.Result.ToList()), ct); await Task.Delay(250, ct);
            UpdateUI(() =>
            {
                SelectedImage = Images?[0];
            });
            await Refresh();
        }

        private async Task Save()
        {
            var images = Images;
            var folder = selectedFolder;
            await UpdateFolderWithPhotoSetAsync(folder);
            await UpdateImagesWithPhotos(images);
            HasSaved();
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
                RaisePropertyChanged(nameof(SelectedBitmapImage));
                RaisePropertyChanged(nameof(FolderDate));
                RaisePropertyChanged(nameof(FolderLocation));
                RaisePropertyChanged(nameof(FolderDescription));
                RaisePropertyChanged(nameof(ImageDescription));
                RaisePropertyChanged(nameof(SelectedBitmapImage));
            });
        }

        private static async Task SendSaveMessage(bool value)
        {
            await Task.Run(() =>
            {
                Messenger.Default.Send<IsSaveMessage>(new IsSaveMessage(value), "Main");
            });
        }

        private ICommand folderInformationEdit;
        private ICommand imageInformationEdit;

        public ICommand FolderInformationEditCommand
        {
            get => folderInformationEdit ??= new RelayCommandAsync(FolderInformationEditAction, true);
        }

        public ICommand ImageInformationEditCommand
        {
            get => imageInformationEdit ??= new RelayCommandAsync(ImageInformationEditAction, true);
        }

        private async Task FolderInformationEditAction()
        {
            await Task.Run(() =>
            {
                FolderInformationEditMode = !FolderInformationEditMode;
            });
        }

        private async Task ImageInformationEditAction()
        {
            await Task.Run(() =>
            {
                ImageInformationEditMode = !ImageInformationEditMode;
            });
        }
    }
}