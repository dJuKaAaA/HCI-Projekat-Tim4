using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.MVVM.ViewModel.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllRestaurantsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestaurantModel> Restaurants { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private RestaurantModel? _selectedRestaurant;

        public RestaurantModel? SelectedRestaurant
        {
            get { return _selectedRestaurant; }
            set { _selectedRestaurant = value; OnPropertyChanged(); }
        }

        private RestaurantSearchPopup? _restaurantSearchPopup;
        private readonly RestaurantSearchViewModel _restaurantSearchViewModel;

        private readonly Service.RestaurantService _restaurantService;
        private readonly Service.NavigationService _navigationService;

        private bool _deleteRestaurantCommandRunning = false;

        public ICommand OpenCreateRestaurantViewComand { get; }
        public ICommand OpenModifyRestaurantViewComand { get; }
        public ICommand DeleteRestaurantCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public AllRestaurantsViewModel(
            Service.RestaurantService restaurantService,
            NavigationService navigationService,
            RestaurantSearchViewModel restaurantSearchViewModel)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            Restaurants = new ObservableCollection<RestaurantModel>();

            _restaurantService = restaurantService;
            _navigationService = navigationService;
            _restaurantSearchViewModel = restaurantSearchViewModel;
            _restaurantSearchViewModel.AllRestaurantsViewModel = this;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenCreateRestaurantViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestaurantViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyRestaurantViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestaurantViewModel>(SelectedRestaurant), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestaurant != null);
            DeleteRestaurantCommand = new Core.RelayCommand(OnDeleteRestaurant, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestaurant != null && !_deleteRestaurantCommandRunning);
            OpenSearchCommand = new RelayCommand(OnOpenSearch, o => true);

            _ = LoadAll();
        }

        private void OnOpenSearch(object o)
        {
            _restaurantSearchPopup?.Close();
            _restaurantSearchPopup = new RestaurantSearchPopup()
            {
                DataContext = _restaurantSearchViewModel
            };
            _restaurantSearchPopup.Show();
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllRestaurantsViewModel))
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.RemoveCUDKeyBindings();
                }
                MainViewModel.RemoveOpenSearchKeyBinding();

                _restaurantSearchPopup?.Close();

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.AddCUDKeyBindings(
                        OpenCreateRestaurantViewComand,
                        OpenModifyRestaurantViewComand,
                        DeleteRestaurantCommand);
                }
                MainViewModel.AddOpenSearchKeyBinding(OpenSearchCommand);
            }

        }

        private async void OnDeleteRestaurant(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this restaurant?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _restaurantService.Delete(SelectedRestaurant.Id);
                await LoadAll();
                MessageBox.Show("Restaurant deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        public async Task LoadAll()
        {
            _deleteRestaurantCommandRunning = true;

            Restaurants.Clear();
            IEnumerable<RestaurantModel> restaurants = await _restaurantService.GetAll();
            foreach (RestaurantModel restaurant in restaurants)
            {
                Restaurants.Add(restaurant);
            }

            _deleteRestaurantCommandRunning = false;
        }
    }
}
