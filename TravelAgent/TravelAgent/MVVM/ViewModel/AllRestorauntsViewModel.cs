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
    public class AllRestorauntsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestorauntModel> Restoraunts { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private RestorauntModel? _selectedRestoraunt;

        public RestorauntModel? SelectedRestoraunt
        {
            get { return _selectedRestoraunt; }
            set { _selectedRestoraunt = value; OnPropertyChanged(); }
        }

        private RestorauntSearchPopup? _restorauntSearchPopup;
        private readonly RestorauntSearchViewModel _restorauntSearchViewModel;

        private readonly Service.RestorauntService _restorauntService;
        private readonly Service.NavigationService _navigationService;

        public ICommand OpenCreateRestorauntViewComand { get; }
        public ICommand OpenModifyRestorauntViewComand { get; }
        public ICommand DeleteRestorauntCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public AllRestorauntsViewModel(
            Service.RestorauntService restorauntService,
            NavigationService navigationService,
            RestorauntSearchViewModel restorauntSearchViewModel)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            Restoraunts = new ObservableCollection<RestorauntModel>();

            _restorauntService = restorauntService;
            _navigationService = navigationService;
            _restorauntSearchViewModel = restorauntSearchViewModel;
            _restorauntSearchViewModel.AllRestorauntsViewModel = this;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenCreateRestorauntViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestorauntViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyRestorauntViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestorauntViewModel>(SelectedRestoraunt), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestoraunt != null);
            DeleteRestorauntCommand = new Core.RelayCommand(OnDeleteRestoraunt, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestoraunt != null);
            OpenSearchCommand = new RelayCommand(OnOpenSearch, o => true);

            Task.Run(async () => await LoadAll());
        }

        private void OnOpenSearch(object o)
        {
            _restorauntSearchPopup?.Close();
            _restorauntSearchPopup = new RestorauntSearchPopup()
            {
                DataContext = _restorauntSearchViewModel
            };
            _restorauntSearchPopup.Show();
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllRestorauntsViewModel))
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.RemoveCUDKeyBindings();
                }
                MainViewModel.RemoveOpenSearchKeyBinding();

                _restorauntSearchPopup?.Close();

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.AddCUDKeyBindings(
                        OpenCreateRestorauntViewComand,
                        OpenModifyRestorauntViewComand,
                        DeleteRestorauntCommand);
                }
                MainViewModel.AddOpenSearchKeyBinding(OpenSearchCommand);
            }

        }

        private async void OnDeleteRestoraunt(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this restoraunt?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _restorauntService.Delete(SelectedRestoraunt.Id);
                await LoadAll();
                MessageBox.Show("Restoraunt deleted successfully!");
            }

        }

        public async Task LoadAll()
        {
            Restoraunts.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetAll();
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                Restoraunts.Add(restoraunt);
            }
        }
    }
}
