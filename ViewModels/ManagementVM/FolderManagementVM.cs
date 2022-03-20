using ImageTool.Comparers;
using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Dialogs;
using ImageTool.Extensions;
using ImageTool.Managements.Interfaces;
using ImageTool.MessageBoxes;
using ImageTool.Objects;
using ImageTool.Struct;
using JSLibrary.Extensions;
using JSLibrary.Logics.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFLibrary.RelayCommands;

namespace ImageTool.ViewModels.ManagementVM
{
    public class FolderManagementVM : EditableViewModelBase
    {
        private string searchText;
        private Folder selectedFolder;

        private List<Folder> filteredFolders;
        private List<Folder> folders;

        private List<Image> images;
        private Image selectedImage;

        private BitmapImage selectedBitmapImage;
        private List<BitmapImage> bitmapImages;

        private readonly ObservableCollection<Exifs> exif = new();

        private bool imageListViewEnable;
        private bool imageViewVisible;

        private bool imageDownloadFailedVisible;
        private bool imageVisible;

        private bool foundedImageView;
        private ObservableCollection<ImageFile> foundedImages;

        private bool imageInformationEditMode = true;
        private bool folderInformationEditMode = true;

        private bool isSaveVisible = true;
        private bool isSaving;

        private bool isAddEnabled;
        private int saveProgress;

        private bool isSaveComplete = true;
        private bool folderDataGridEnabled = true;

        private bool isNew;
        private bool isCloseButtonEnabled = true;

        private bool isRepeatUploadImagesButtonEnabled = true;
        private bool isUploadImageButtonEnabled = true;

        private Action<bool> dialogResultAction;

        private ICommand close;
        private ICommand refresh;
        private ICommand add;
        private ICommand remove;
        private ICommand save;
        private ICommand uploadImages;
        private ICommand repeatUploadImages;
        private ICommand folderInformationEdit;
        private ICommand imageInformationEdit;

        public FolderManagementVM(
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
            LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
            NewFolder().ContinueWith(x =>
            {
                IsSaveComplete = true;
            });
            IsAddEnabled = true;
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                RaisePropertyChanged(nameof(SearchText));
            }
        }

        public Folder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                if (value != null)
                {
                    SaveCheck();
                    selectedFolder = value;
                    Task.Run(async () => await SelectedFolderCheck(selectedFolder));
                    RaisePropertyChanged(nameof(SelectedFolder));
                }
            }
        }

        public List<Folder> Folders
        {
            get => folders;
            set
            {
                folders = value;
                FilteredFolders = Folders;
                RaisePropertyChanged(nameof(Folders));
            }
        }

        public List<Folder> FilteredFolders
        {
            get => filteredFolders;
            set
            {
                filteredFolders = value;
                RaisePropertyChanged(nameof(FilteredFolders));
            }
        }

        public ObservableCollection<ImageFile> FoundedImages
        {
            get => foundedImages;
            set
            {
                foundedImages = value;
                RaisePropertyChanged(nameof(FoundedImages));
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

        public BitmapImage SelectedBitmapImage
        {
            get => selectedBitmapImage;
            set
            {
                selectedBitmapImage = value;
                RaisePropertyChanged(nameof(SelectedBitmapImage));
            }
        }

        public ObservableCollection<Exifs> Exif
        {
            get => exif;
        }

        public bool FolderDataGridEnabled
        {
            get => folderDataGridEnabled;
            set
            {
                folderDataGridEnabled = value;
                RaisePropertyChanged(nameof(FolderDataGridEnabled));
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
                if (value != null && SelectedFolder != null)
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

        public bool FolderInformationEditMode
        {
            get => folderInformationEditMode;
            set
            {
                folderInformationEditMode = value;
                RaisePropertyChanged(nameof(FolderInformationEditMode));
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

        public bool ImageListViewEnable
        {
            get => imageListViewEnable;
            set
            {
                imageListViewEnable = value;
                RaisePropertyChanged(nameof(ImageListViewEnable));
            }
        }

        public bool ImageViewVisible
        {
            get => imageViewVisible;
            set
            {
                imageViewVisible = value;
                RaisePropertyChanged(nameof(ImageViewVisible));
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

        public bool FoundedImageView
        {
            get => foundedImageView;
            set
            {
                foundedImageView = value;
                RaisePropertyChanged(nameof(FoundedImageView));
            }
        }

        public bool IsAddEnabled
        {
            get => isAddEnabled;
            set
            {
                isAddEnabled = value;
                RaisePropertyChanged(nameof(IsAddEnabled));
            }
        }

        public bool IsSaving
        {
            get => isSaving;
            set
            {
                isSaving = value;
                RaisePropertyChanged(nameof(IsSaving));
            }
        }

        public bool IsSaveVisible
        {
            get => isSaveVisible;
            set
            {
                isSaveVisible = value;
                RaisePropertyChanged(nameof(IsSaveVisible));
            }
        }

        public int SaveProgress
        {
            get => saveProgress;
            set
            {
                saveProgress = value;
                RaisePropertyChanged(nameof(SaveProgress));
            }
        }

        public bool IsSaveComplete
        {
            get => isSaveComplete;
            set
            {
                isSaveComplete = value;
                RaisePropertyChanged(nameof(IsSaveComplete));
            }
        }

        public bool IsCloseButtonEnabled
        {
            get => isCloseButtonEnabled;
            set
            {
                isCloseButtonEnabled = value;
                RaisePropertyChanged(nameof(IsCloseButtonEnabled));
            }
        }

        public bool IsUploadImagesButtonEnabled
        {
            get => isUploadImageButtonEnabled;
            set
            {
                isUploadImageButtonEnabled = value;
                RaisePropertyChanged(nameof(IsUploadImagesButtonEnabled));
            }
        }

        public bool IsRepeatUploadImagesButtonEnabled
        {
            get => isRepeatUploadImagesButtonEnabled;
            set
            {
                isRepeatUploadImagesButtonEnabled = value;
                RaisePropertyChanged(nameof(IsRepeatUploadImagesButtonEnabled));
            }
        }

        public Action<bool> DialogResultAction { get => dialogResultAction; set => dialogResultAction = value; }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            SaveCheck();
        }

        public override void Dispose()
        {
            images?.Clear();
            bitmapImages?.Clear();
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private void LockUI()
        {
            FolderDataGridEnabled = false;
            IsAddEnabled = false;
            IsSaveVisible = false;
            IsCloseButtonEnabled = false;
            IsUploadImagesButtonEnabled = false;
            IsRepeatUploadImagesButtonEnabled = false;
        }

        private void UnlockUI()
        {
            FolderDataGridEnabled = true;
            IsAddEnabled = true;
            IsSaveVisible = true;
            IsCloseButtonEnabled = true;
            IsUploadImagesButtonEnabled = true;
            IsRepeatUploadImagesButtonEnabled = true;
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
            if (IsSaveComplete)
            {
                IsSaveComplete = !IsSaveComplete;
            }
        }

        private void HasSaved()
        {
            if (isNew == true)
            {
                isNew = false;
            }
            if (!IsSaveComplete)
            {
                IsSaveComplete = !IsSaveComplete;
            }
        }

        private void SaveCheck()
        {
            if (!IsSaveComplete)
            {
                CustomMessageBox.SaveMessageBox(async () => await Save(), () => HasSaved());
            }
        }

        private void UploadImages(IEnumerable<ImageFile> imageFiles, Folder folder)
        {
            LockUI();
            if (folder?.Id != null)
            {
                try
                {
                    CustomMessageBox.UploadMessagebox(async () =>
                    {
                        List<Image> newImages = await CreateImagesWithUpload(imageFiles);
                        bool successfully = await UploadSuccessfully(newImages, imageFiles.Count());
                        if (successfully)
                        {
                            CustomMessageBox.SaveMessageBox(async () => await SaveUploadImages(newImages), async () => await NoSaveUploadImages(newImages));
                        }
                    });
                }
                catch (Exception e)
                {
                    UnlockUI();
                    CustomMessageBox.ErrorRepeatMessageBox(e.Message, () =>
                    { }, () => GC.Collect());
                }
            }
        }

        private async Task SelectedFolderCheck(Folder folder)
        {
            if (folder != null && folder.Id != 0)
            {
                bitmapImages?.Clear();
                await LoadCompleteFolder(folder, ct);
            }
        }

        private async Task LoadCompleteFolder(Folder folder, CancellationToken cancellationToken = default)
        {
            int folderId = folder.Id;
            await FolderBL.GetAsync(folderId, cancellationToken).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => selectedFolder = t.Result), cancellationToken);
            await LoadImages(folderId, cancellationToken).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => images = t.Result.ToList()), cancellationToken);
            Images?.Sort(new ImageComparer());
            await RefreshWithoutBitmapImage();
            if (bitmapImages?.Count > 0)
            {
                bitmapImages.Clear();
            }
            await LoadBitmapImages(Images, cancellationToken, () =>
            {
                ImageListViewEnable = false;
                ImageDownloadFailedVisible = false;
                ImageVisible = false;
            }, () =>
            {
                ImageListViewEnable = true;
                ImageVisible = true;
            }).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => bitmapImages = t.Result.ToList()), cancellationToken);
            await Task.Delay(250, cancellationToken);
            FoundedImageView = false;
            ImageViewVisible = true;
            if (Images?.Count > 0)
            {
                SelectedImage = Images?[0];
            }
            await Refresh();
        }

        private async Task Save()
        {
            var selectedFolder = SelectedFolder;
            var images = Images;
            if (isNew)
            {
                await FolderBL.AddAsync(selectedFolder);
                isNew = false;
                IsAddEnabled = true;
            }
            else
            {
                await UpdateFolderWithPhotoSetAsync(selectedFolder);
            }
            if (images != null)
            {
                await UpdateImagesWithPhotos(images);
            }
        }

        private async Task Delete(int folderId)
        {
            Folder folder = await FolderBL.GetAsync(folderId, ct);
            string photoSetId = folder.PhotoSetId;
            List<FolderImageLink> folderImageLinks = folder.FolderImageLinks.ToList();
            List<Image> images = folderImageLinks.Select(x => x.Image).ToList();
            List<string> photoIds = images.Select(x => x.PhotoId).ToList();
            List<Exif> exifs = images.Select(x => x.Exif).ToList();

            await FolderImageLinkBL.DeleteManyAsync(folderImageLinks, ct);

            await FolderBL.DeleteAsync(folder);

            await ImageBL.DeleteManyAsync(images, ct);

            await DeletePhotos(photoIds);

            await DeletePhotoSet(photoSetId);

            await ExifBL.DeleteManyAsync(exifs, ct);

            await LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
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

        private async Task Reload()
        {
            SaveCheck();
            var selectedFolder = SelectedFolder;
            await LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
            SelectedFolder = selectedFolder;
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

        private async Task NewFolder()
        {
            SaveCheck();
            SelectedFolder = new Folder();
            FoundedImages = null;
            Images = null;
            Exif.Clear();
            isNew = true;
            IsAddEnabled = false;
            ImageViewVisible = false;
            FoundedImageView = true;
            await ImportFolder();
        }

        private async Task ImportFolder()
        {
            string folderpath = await OpenFolder.DialogAsync();
            if (folderpath != null)
            {
                IDictionary<string, string> dicionary = FileManagement.GetFoldernameSplited(folderpath);
                if (!ExistsFolder(dicionary, Folders))
                {
                    FolderLocation = dicionary["Location"];
                    FolderDate = dicionary["Date"];
                    FoundedImages = await GetImageFilesAsync(folderpath);
                }
                else
                {
                    CustomMessageBox.InformationMessageBox("Dieser Ordner ist schon vorhanden");
                }
            }
        }

        private async Task SaveUploadImages(List<Image> images)
        {
            (var photoIds, var primaryPhotoId) = images.GetPhotoSetItems();
            selectedFolder.PhotoSetId = await CreatePhotoSetIdWithPrimaryPhotoId(FolderLocation + "_" + FolderDate, primaryPhotoId);
            await AddPhotosToPhotoSetAsync(photoIds, selectedFolder.PhotoSetId);
            await AddPhotosToPhotoSetAsync(photoIds, selectedFolder.PhotoSetId);
            var folder = selectedFolder;
            await FolderBL.AddAsync(folder);
            var newImageIds = await AddImagesWithExifsAsync(images);
            await FolderImageLinkBL.AddManyAsync(await CreateFolderImageLinks(folder.Id, newImageIds), ct);
            HasSaved();
            GC.Collect();
            UpdateUI(async () =>
            {
                await LoadCompleteFolder(selectedFolder, ct);
                await LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
                UnlockUI();
            });
        }

        private async Task NoSaveUploadImages(List<Image> images)
        {
            List<string> photoIds = images.Select(x => x.PhotoId).ToList();
            await DeletePhotos(photoIds);
            HasSaved();
            UnlockUI();
            UpdateUI(async () =>
            {
                await NewFolder();
            });
        }

        private async Task<bool> UploadSuccessfully(List<Image> images, int number)
        {
            if (images.Count != number)
            {
                CustomMessageBox.InformationMessageBox("Leider würde nicht alle Bilder erfolgreich hochgeladen.");
                List<string> photoIds = images.Select(x => x.PhotoId).ToList();
                await DeletePhotos(photoIds);
                HasSaved();
                UnlockUI();
                return false;
            }
            return true;
        }

        public ICommand CloseCommand
        {
            get => close ??= new RelayCommandAsync(CloseAction, true);
        }

        public ICommand RefreshCommand
        {
            get => refresh ??= new RelayCommandAsync(RefreshAction, true);
        }

        public ICommand AddCommand
        {
            get => add ??= new RelayCommandAsync(AddAction, true);
        }

        public ICommand RemoveCommand
        {
            get => remove ??= new RelayCommand((param) => RemoveAction(param), (param) => true);
        }

        public ICommand SaveCommand
        {
            get => save ??= new RelayCommandAsync(SaveAction, true);
        }

        public ICommand UploadImagesCommand
        {
            get => uploadImages ??= new RelayCommand(UploadImagesAction, true);
        }

        public ICommand RepeatUploadImagesCommand
        {
            get => repeatUploadImages ??= new RelayCommand(RepeatUploadImagesAction, true);
        }

        public ICommand FolderInformationEditCommand
        {
            get => folderInformationEdit ??= new RelayCommandAsync(FolderInformationEditAction, true);
        }

        public ICommand ImageInformationEditCommand
        {
            get => imageInformationEdit ??= new RelayCommandAsync(ImageInformationEditAction, true);
        }

        private async Task CloseAction()
        {
            await UpdateUIAsync(() =>
            {
                DialogResultAction?.Invoke(true);
            });
        }

        private async Task RefreshAction()
        {
            await Reload();
        }

        private async Task AddAction()
        {
            await NewFolder();
        }

        private void RemoveAction(object obj)
        {
            int? input = obj as int?;
            if (input.HasValue)
            {
                int id = input.Value;
                CustomMessageBox.DeleteMessageBox(async () =>
                {
                    await Delete(id);
                }, () => { });
            }
        }

        private async Task SaveAction()
        {
            await Save();
        }

        private void UploadImagesAction()
        {
            UploadImages(FoundedImages, SelectedFolder);
            IsAddEnabled = true;
        }

        private void RepeatUploadImagesAction()
        {
            CustomMessageBox.RepeatMessageBox(async () =>
            {
                await ImportFolder();
            }, () =>
            {
                IsUploadImagesButtonEnabled = false;
                IsRepeatUploadImagesButtonEnabled = false;
            });
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