using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using TravelAgent.Core;
using TravelAgent.MVVM.ViewModel;

namespace TravelAgent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<MainWindow>(provider => new MainWindow()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<Consts>();

            // makes navigation from one ViewModel to another possible
            services.AddTransient<Func<Type, ViewModel>>(provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType));

            // register ViewModel classes for injection here
            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<DeleteLaterViewModel>();
            services.AddTransient<LoginViewModel>();

            // register Service classes for injection here
            services.AddSingleton<Service.NavigationService>();
            services.AddSingleton<Service.DatabaseExcecutionService>();
            services.AddSingleton<Service.UserService>();

            // setting the SQLite provider
            SQLitePCL.Batteries.Init();
            SQLitePCL.raw.SetProvider(new SQLite3Provider_e_sqlite3());

            _serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
