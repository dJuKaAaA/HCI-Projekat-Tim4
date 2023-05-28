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

namespace TravelAgent.MVVM.ViewModel
{
    public class AllAccommodationsViewModel : Core.ViewModel
    {
        public ObservableCollection<AccommodationModel> Accommodations { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private AccommodationModel? _selectedAccommodation;

        public AccommodationModel? SelectedAccommodation
        {
            get { return _selectedAccommodation; }
            set { _selectedAccommodation = value; OnPropertyChanged(); }
        }

        private AccommodationSearchPopup? _accommodationSearchPopup;
        private readonly AccommodationSearchViewModel _accommodationSearchViewModel;

        private readonly Service.AccommodationService _accommodationService;
        private readonly Service.NavigationService _navigationService;

        public ICommand OpenCreateAccommodationViewComand { get; }
        public ICommand OpenModifyAccommodationViewComand { get; }
        public ICommand DeleteAccommodationCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public AllAccommodationsViewModel(
            Service.AccommodationService acccommodationService, 
            Service.NavigationService navigationService,
            AccommodationSearchViewModel accommodationSearchViewModel)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            Accommodations = new ObservableCollection<AccommodationModel>();

            _accommodationService = acccommodationService;
            _navigationService = navigationService;
            _accommodationSearchViewModel = accommodationSearchViewModel;
            _accommodationSearchViewModel.AllAccommodationsViewModel = this;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenCreateAccommodationViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateAccommodationViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyAccommodationViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateAccommodationViewModel>(SelectedAccommodation), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedAccommodation != null);
            DeleteAccommodationCommand = new Core.RelayCommand(OnDeleteAccommodation, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedAccommodation != null);
            OpenSearchCommand = new RelayCommand(OnOpenSearch, o => true);

            Task.Run(async () => await LoadAll());
        }

        private void OnOpenSearch(object o)
        {
            _accommodationSearchPopup?.Close();
            _accommodationSearchPopup = new AccommodationSearchPopup()
            {
                DataContext = _accommodationSearchViewModel
            };
            _accommodationSearchPopup.Show();
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllAccommodationsViewModel))
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.RemoveCUDKeyBindings();
                }
                MainViewModel.RemoveOpenSearchKeyBinding();

                _accommodationSearchPopup?.Close();

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.AddCUDKeyBindings(
                        OpenCreateAccommodationViewComand,
                        OpenModifyAccommodationViewComand,
                        DeleteAccommodationCommand);
                }
                MainViewModel.AddOpenSearchKeyBinding(OpenSearchCommand);
            }

        }

        private async void OnDeleteAccommodation(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this accommodation?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _accommodationService.Delete(SelectedAccommodation.Id);
                await LoadAll();
                MessageBox.Show("Accommodation deleted successfully!");
            }
        }

        public async Task LoadAll()
        {
            Accommodations.Clear();
            IEnumerable<AccommodationModel> accommodations = await _accommodationService.GetAll();
            foreach (AccommodationModel accommodation in accommodations)
            {
                Accommodations.Add(accommodation);
            }
        }

    }
}
