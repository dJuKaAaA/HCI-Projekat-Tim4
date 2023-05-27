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
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllTouristAttractionsViewModel : Core.ViewModel
    {
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private TouristAttractionModel? _selectedTouristAttraction;

        public TouristAttractionModel? SelectedTouristAttraction
        {
            get { return _selectedTouristAttraction; }
            set { _selectedTouristAttraction = value; OnPropertyChanged(); }
        }

        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.NavigationService _navigationService;

        public ICommand OpenCreateTouristAttractionViewComand { get; }
        public ICommand OpenModifyTouristAttractionViewComand { get; }
        public ICommand DeleteTouristAttractionCommand { get; }

        public AllTouristAttractionsViewModel(
            Service.TouristAttractionService touristAttractionService, 
            NavigationService navigationService)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();

            _touristAttractionService = touristAttractionService;
            _navigationService = navigationService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenCreateTouristAttractionViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTouristAttractionViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyTouristAttractionViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTouristAttractionViewModel>(SelectedTouristAttraction), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTouristAttraction != null);
            DeleteTouristAttractionCommand = new Core.RelayCommand(OnDeleteRestoraunt, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTouristAttraction != null);
            LoadAll();
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllTouristAttractionsViewModel))
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.RemoveCUDKeyBindings();
                }

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.AddCUDKeyBindings(
                        OpenCreateTouristAttractionViewComand,
                        OpenModifyTouristAttractionViewComand,
                        DeleteTouristAttractionCommand);
                    
                }
            }

        }

        private async void OnDeleteRestoraunt(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this restoraunt?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _touristAttractionService.Delete(SelectedTouristAttraction.Id);
                LoadAll();
                MessageBox.Show("Tourist attraction deleted successfully!");
            }

        }

        private async void LoadAll()
        {
            AllTouristAttractions.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await _touristAttractionService.GetAll();
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                AllTouristAttractions.Add(touristAttraction);
            }
        }

    }
}
