using ImageTool.BusinessLogics;
using ImageTool.DBContexts;
using ImageTool.DBModels;
using ImageTool.Helpers;
using ImageTool.Helpers.Interfaces;
using ImageTool.Managements;
using ImageTool.Managements.Interfaces;
using ImageTool.Settings;
using ImageTool.Settings.Interfaces;
using ImageTool.Stores;
using ImageTool.Stores.Interfaces;
using ImageTool.Uploaders;
using ImageTool.Uploaders.Interfaces;
using ImageTool.ViewModels.MainVM;
using ImageTool.ViewModels.ManagementVM;
using ImageTool.Views.Main;
using ImageTool.Views.Management;
using ImageTool.Windows;
using JSLibrary.Logics.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageTool.Extensions
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Settings");

            services.AddSingleton<IFlickrSettings, FlickrSettings>(x =>
            {
                FlickrSettings flickrSettings = new();
                section.Bind("FlickrSettings", flickrSettings);
                return flickrSettings;
            });
            return services;
        }

        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddTransient<IFlickrAuthHelper, FlickrAuthHelper>();
            return services;
        }

        public static IServiceCollection AddUploaders(this IServiceCollection services)
        {
            services.AddTransient<IFlickrUploader, FlickrUploader>();
            return services;
        }

        public static IServiceCollection AddBL(this IServiceCollection services)
        {
            services.AddSingleton<IBusinessLogicBase<Folder, DBContext>, FolderBusinessLogic>();
            services.AddSingleton<IBusinessLogicBase<Image, DBContext>, ImageBusinessLogic>();
            services.AddSingleton<IBusinessLogicBase<Exif, DBContext>, ExifBusinessLogic>();
            services.AddSingleton<IBusinessLogicBase<FolderImageLink, DBContext>, FolderImageLinkBusinessLogic>();
            return services;
        }

        public static IServiceCollection AddManagements(this IServiceCollection services)
        {
            services.AddSingleton<IFlickrManagement, FlickrManagement>();
            services.AddSingleton<IBitmapImageManagement, BitmapImageManagement>();
            services.AddSingleton<IExifManagement, ExifManagement>();
            services.AddSingleton<IFileManagement, FileManagement>();
            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddSingleton<EditMainUC>();
            services.AddSingleton<ViewMainUC>();
            services.AddSingleton<SearchMainUC>();
            services.AddSingleton<FolderManagementUC>();
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<FolderManagementVM>();
            services.AddTransient<SearchMainVM>();
            services.AddTransient<EditMainVM>();
            services.AddTransient<ViewMainVM>();
            return services;
        }

        public static IServiceCollection AddWindows(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ManagementWindow>();
            return services;
        }

        public static IServiceCollection AddStores(this IServiceCollection services)
        {
            services.AddSingleton<INavigationStore, NavigationStore>();
            return services;
        }
    }
}