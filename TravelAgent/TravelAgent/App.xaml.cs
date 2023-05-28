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
using TravelAgent.MVVM.View.Popup;
using TravelAgent.MVVM.ViewModel;
using TravelAgent.MVVM.ViewModel.Popup;

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

            services.AddSingleton<MainWindow>(provider => new MainWindow()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<Consts>();

            // makes navigation from one ViewModel to another possible
            services.AddTransient<Func<Type, ViewModel>>(provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType));

            // register ViewModel classes for injection here (use AddTransient)
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<AllTripsViewModel>();
            services.AddTransient<AllTouristAttractionsViewModel>();
            services.AddTransient<AllRestorauntsViewModel>();
            services.AddTransient<AllAccommodationsViewModel>();
            services.AddTransient<CreateTripViewModel>();
            services.AddTransient<CreateAccommodationViewModel>();
            services.AddTransient<CreateRestorauntViewModel>();
            services.AddTransient<CreateTouristAttractionViewModel>();
            services.AddTransient<UserTripsViewModel>();
            services.AddTransient<MapViewModel>();
            services.AddTransient<SeeDealViewModel>();
            services.AddTransient<UserTripSearchViewModel>();
            services.AddTransient<AccommodationSearchViewModel>();
            services.AddTransient<RestorauntSearchViewModel>();
            services.AddTransient<TouristAttractionSearchViewModel>();
            services.AddTransient<TripSearchViewModel>();

            // register Service classes for injection here (use AddSingleton)
            services.AddSingleton<Service.NavigationService>();
            services.AddSingleton<Service.DatabaseExecutionService>();
            services.AddSingleton<Service.UserService>();
            services.AddSingleton<Service.LocationService>();
            services.AddSingleton<Service.TouristAttractionService>();
            services.AddSingleton<Service.TripService>();
            services.AddSingleton<Service.RestorauntService>();
            services.AddSingleton<Service.AccommodationService>();
            services.AddSingleton<Service.UserTripService>();
            services.AddSingleton<Service.MapService>();
            services.AddSingleton<Service.ImageService>();

            _serviceProvider = services.BuildServiceProvider();

            // setting the SQLite provider
            SQLitePCL.Batteries.Init();
            SQLitePCL.raw.SetProvider(new SQLite3Provider_e_sqlite3());

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Perform any cleanup or additional logic before the application exits here

            base.OnExit(e);
        }
    }
}
