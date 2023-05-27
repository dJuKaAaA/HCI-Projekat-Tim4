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
    public class AllRestorauntsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }

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

        private readonly Service.RestorauntService _restorauntService;
        private readonly Service.NavigationService _navigationService;

        public ICommand OpenCreateRestorauntViewComand { get; }
        public ICommand OpenModifyRestorauntViewComand { get; }
        public ICommand DeleteRestorauntCommand { get; }

        public AllRestorauntsViewModel(
            Service.RestorauntService restorauntService,
            NavigationService navigationService)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            AllRestoraunts = new ObservableCollection<RestorauntModel>();

            _restorauntService = restorauntService;
            _navigationService = navigationService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenCreateRestorauntViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestorauntViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyRestorauntViewComand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateRestorauntViewModel>(SelectedRestoraunt), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestoraunt != null);
            DeleteRestorauntCommand = new Core.RelayCommand(OnDeleteRestoraunt, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedRestoraunt != null);
            LoadAll();
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllRestorauntsViewModel))
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
                        OpenCreateRestorauntViewComand,
                        OpenModifyRestorauntViewComand,
                        DeleteRestorauntCommand);
                    
                }
            }

        }

        private async void OnDeleteRestoraunt(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this restoraunt?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _restorauntService.Delete(SelectedRestoraunt.Id);
                LoadAll();
                MessageBox.Show("Restoraunt deleted successfully!");
            }

        }

        private async void LoadAll()
        {
            AllRestoraunts.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetAll();
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                AllRestoraunts.Add(restoraunt);
            }
        }
    }
}
