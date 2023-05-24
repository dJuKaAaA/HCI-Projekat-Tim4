using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllAccommodationsViewModel : Core.ViewModel
    {
        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private readonly Service.AccommodationService _accommodationService;

        public AllAccommodationsViewModel(Service.AccommodationService acccommodationService)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            AllAccommodations = new ObservableCollection<AccommodationModel>();

            _accommodationService = acccommodationService;

            LoadAll();

        }

        private async void LoadAll()
        {
            AllAccommodations.Clear();
            IEnumerable<AccommodationModel> accommodations = await _accommodationService.GetAll();
            foreach (AccommodationModel accommodation in accommodations)
            {
                AllAccommodations.Add(accommodation);
            }
        }

    }
}
