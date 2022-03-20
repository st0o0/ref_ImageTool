using ImageTool.Stores.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageTool.Stores
{
    public class NavigationStore : INavigationStore
    {
        public NavigationStore(IServiceProvider service)
        {
            this.Service = service;
        }

        public IServiceProvider Service { get; init; }

        public UserControl GetUserControl<T>() where T : UserControl
        {
            return Service.GetRequiredService<T>();
        }

        public Window GetWindow<T>() where T : Window
        {
            return Service.GetRequiredService<T>();
        }
    }
}