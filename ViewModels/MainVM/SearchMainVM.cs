using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Managements.Interfaces;
using ImageTool.MessageBoxes;
using ImageTool.Messages;
using ImageTool.Stores.Interfaces;
using ImageTool.Views.Main;
using ImageTool.Windows;
using JSLibrary.Logics.Business.Interfaces;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFLibrary.RelayCommands;
using Image = ImageTool.DBModels.Image;

namespace ImageTool.ViewModels.MainVM
{
    public class SearchMainVM : ViewableViewModelBase
    {
        private const string EditToken = "MainEdit";
        private const string ViewToken = "MainView";

        private List<Folder> filteredFolders;
        private List<Folder> folders;

        private Folder selectedFolder;
        private string selectedToken = "";
        private UserControl selectedUserControl = null;

        private bool isRefreshing;
        private bool isSaving;

        private bool isSaveVisible = false;
        private bool isSaveComplete = true;

        private bool isEditMode = false;
        private string searchText = "";

        private int refreshProgress;
        private int saveProgress;

        private readonly INavigationStore navigationStore;
        private readonly IFlickrManagement flickrManagement;

        private ICommand refresh;
        private ICommand management;
        private ICommand edit;
        private ICommand save;

        public SearchMainVM(
            INavigationStore navigationStore,
            IFlickrManagement flickrManagement,
            IBitmapImageManagement bitmapImageManagement,
            IBusinessLogicBase<Folder, DBContext> folderBL,
            IBusinessLogicBase<Image, DBContext> imageBL,
            IBusinessLogicBase<Exif, DBContext> exifBL,
            IBusinessLogicBase<FolderImageLink, DBContext> folderImageLinkBL) : base(bitmapImageManagement, folderBL, imageBL, exifBL, folderImageLinkBL)
        {
            this.navigationStore = navigationStore;
            this.flickrManagement = flickrManagement;
            Messenger.Default.Register<IsSaveMessage>(this, "Main", (ISM) =>
            {
                IsSaveComplete = ISM.IsSaved;
            });
            LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
            FlickrCheck();
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                SetFilter(SearchText, Folders, (t) => FilteredFolders = t.ToList());
                RaisePropertyChanged(nameof(SearchText));
            }
        }

        public List<Folder> Folders
        {
            get => folders;
            set
            {
                folders = value;
                FilteredFolders = folders;
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

        public Folder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                SaveCheck();
                selectedFolder = value;
                SelectedFolderCheck();
                RaisePropertyChanged(nameof(SelectedFolder));
            }
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                RaisePropertyChanged(nameof(IsRefreshing));
                RaisePropertyChanged(nameof(IsListViewEnable));
            }
        }

        public bool IsSaving
        {
            get => isSaving;
            set
            {
                isSaving = value;
                RaisePropertyChanged(nameof(IsSaving));
                RaisePropertyChanged(nameof(IsEditButtonEnable));
                RaisePropertyChanged(nameof(IsListViewEnable));
            }
        }

        public int RefreshProgress
        {
            get => refreshProgress;
            set
            {
                refreshProgress = value;
                RaisePropertyChanged(nameof(RefreshProgress));
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

        public bool IsSaveVisible
        {
            get => isSaveVisible;
            set
            {
                isSaveVisible = value;
                RaisePropertyChanged(nameof(IsSaveVisible));
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

        public bool IsListViewEnable
        {
            get => !IsRefreshing;
        }

        public bool IsEditButtonEnable
        {
            get => !IsSaving;
        }

        public bool IsEditMode
        {
            get => isEditMode;
            set
            {
                isEditMode = value;
                RaisePropertyChanged(nameof(IsEditMode));
            }
        }

        public UserControl SelectedUserControl
        {
            get => selectedUserControl;
            set
            {
                selectedUserControl = value;
                RaisePropertyChanged(nameof(SelectedUserControl));
            }
        }

        public Action CloseAction { get; set; }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            SaveCheck();
            this.Cancel();
            this.Dispose();
        }

        private void ChangeSaveProgress(int value)
        {
            if (value == 1)
            {
                IsSaving = true;
            }
            if (value == 100)
            {
                IsSaving = false;
            }
            SaveProgress = value;
        }

        private void ChangeRefreshProgress(int value)
        {
            if (value == 1)
            {
                IsRefreshing = true;
            }
            if (value == 100)
            {
                IsRefreshing = false;
            }
            RefreshProgress = value;
        }

        private void FlickrCheck()
        {
            var s = this.flickrManagement.TestLogin();
            MessageBox.Show("Username:\n" + s, "Flickr Check", MessageBoxButton.OK);
        }

        private void SaveCheck()
        {
            if (!IsSaveComplete)
            {
                CustomMessageBox.SaveMessageBox(async () => await SaveAsync(), async () => await SendSaveCommandMessage(false));
            }
        }

        private void SelectedFolderCheck()
        {
            UpdateUI(async () =>
            {
                await ChangeUserControl();
            });
        }

        private async Task SendSelectedFolderMessage(Folder folder)
        {
            if (folder != null)
            {
                await Task.Run(() =>
                {
                    Messenger.Default.Send<SelectedFolderMessage>(new SelectedFolderMessage(folder), selectedToken);
                });
            }
        }

        private async Task SendSaveCommandMessage(bool value)
        {
            await Task.Run(() =>
            {
                Messenger.Default.Send<SaveCommandMessage>(new SaveCommandMessage(value), selectedToken);
            });
        }

        private async Task SendRefreshCommandMessage()
        {
            await Task.Run(() =>
            {
                Messenger.Default.Send<RefreshCommandMessage>(new RefreshCommandMessage(true), selectedToken);
            });
        }

        private async Task ChangeUserControl()
        {
            if (IsEditMode)
            {
                if (selectedToken != EditToken)
                {
                    IsSaveVisible = true;
                    selectedToken = EditToken;
                    SelectedUserControl = navigationStore.GetUserControl<EditMainUC>();
                }
            }
            else
            {
                if (selectedToken != ViewToken)
                {
                    IsSaveVisible = false;
                    selectedToken = ViewToken;
                    SelectedUserControl = navigationStore.GetUserControl<ViewMainUC>();
                }
            }
            await SendSelectedFolderMessage(SelectedFolder);
        }

        private async Task RefreshAsync()
        {
            Task refreshprogress = ProgressSimulation(ChangeRefreshProgress, 25);
            await LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
            await SendRefreshCommandMessage();
            await refreshprogress;
        }

        private async Task SaveAsync()
        {
            Task saveprogress = ProgressSimulation(ChangeSaveProgress, 125);
            await SendSaveCommandMessage(true);
            await saveprogress;
        }

        public ICommand RefreshCommand
        {
            get => refresh ??= new RelayCommandAsync(RefreshAction, true);
        }

        public ICommand ManagementCommand
        {
            get => management ??= new RelayCommand(ManagementAction, true);
        }

        public ICommand EditCommand
        {
            get => edit ??= new RelayCommandAsync(EditAction, true);
        }

        public ICommand SaveCommand
        {
            get => save ??= new RelayCommandAsync(SaveAction, true);
        }

        private async Task RefreshAction()
        {
            await RefreshAsync();
        }

        private void ManagementAction()
        {
            var t = navigationStore.GetWindow<ManagementWindow>();
            t.Focus();
            bool? result = t.ShowDialog();
            if (result.Value == true)
            {
            }
            var sFolder = SelectedFolder;
            LoadFolders(ct).ContinueWith(x => IsTaskCompleteSuccessfully(x, (t) => Folders = t.Result.ToList()));
            if (sFolder != null)
            {
                SelectedFolder = sFolder;
            }
        }

        private async Task EditAction()
        {
            IsEditMode = !IsEditMode;
            SaveCheck();
            await ChangeUserControl();
        }

        private async Task SaveAction()
        {
            await SaveAsync();
        }
    }
}